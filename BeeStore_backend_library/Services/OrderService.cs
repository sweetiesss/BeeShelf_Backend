using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.OrderDTOs;
using BeeStore_Repository.Enums;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BeeStore_Repository.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        private async Task<List<Order>> ApplyFilterToList(OrderStatus? orderStatus, OrderSortBy? sortCriteria,
                                                          bool descending, int? shipperId = null, int? userId = null, int? warehouseId = null)
        {
            string? filterQuery = orderStatus switch
            {
                OrderStatus.Draft => Constants.Status.Draft,
                OrderStatus.Pending => Constants.Status.Pending,
                OrderStatus.Processing => Constants.Status.Processing,
                OrderStatus.Shipping => Constants.Status.Shipping,
                OrderStatus.Delivered => Constants.Status.Delivered,
                OrderStatus.Completed => Constants.Status.Completed,
                OrderStatus.Returned => Constants.Status.Returned,
                OrderStatus.Refunded => Constants.Status.Refunded,
                OrderStatus.Canceled => Constants.Status.Canceled,
                _ => null
            };

            string? sortBy = sortCriteria switch
            {
                OrderSortBy.CreateDate => Constants.SortCriteria.CreateDate,
                OrderSortBy.TotalPrice => Constants.SortCriteria.TotalPrice,
                OrderSortBy.ProductAmount => Constants.SortCriteria.ProductAmount,
                _ => null
            };

            var list = await _unitOfWork.OrderRepo.GetListAsync(
                filter: u => (filterQuery == null || u.Status.Equals(filterQuery))
                             && (userId == null || u.OcopPartnerId.Equals(userId))
                             && (warehouseId == null || u.OrderDetails.Any(od => od.Lot.Inventory.WarehouseId.Equals(warehouseId)))
                             && (shipperId == null || u.Batch.BatchDeliveries.Any(u => u.DeliverBy.Equals(shipperId) && u.IsDeleted.Equals(false))),
                includes: null,
                sortBy: sortBy!,
                descending: descending,
                searchTerm: null,
                searchProperties: null
                );
            return list;
        }

        public async Task<Pagination<OrderListDTO>> GetOrderList(OrderStatus? orderStatus, OrderSortBy? sortCriteria,
                                                          bool descending, int pageIndex, int pageSize)
        {
            var list = await ApplyFilterToList(orderStatus, sortCriteria, descending);
            var result = _mapper.Map<List<OrderListDTO>>(list);
            return await ListPagination<OrderListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<Pagination<OrderListDTO>> GetWarehouseSentOrderList(int warehouseId, OrderStatus? orderStatus, OrderSortBy? sortCriteria, bool descending, int pageIndex, int pageSize)
        {
            var list = await ApplyFilterToList(orderStatus, sortCriteria, descending, null, null, warehouseId);
            var result = _mapper.Map<List<OrderListDTO>>(list);
            return await ListPagination<OrderListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<Pagination<OrderListDTO>> GetOrderList(int userId, OrderStatus? orderStatus, OrderSortBy? sortCriteria,
                                                          bool descending, int pageIndex, int pageSize)
        {
            var list = await ApplyFilterToList(orderStatus, sortCriteria, descending, null, userId);
            var result = _mapper.Map<List<OrderListDTO>>(list);
            return await ListPagination<OrderListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<Pagination<OrderListDTO>> GetDeliverOrderList(int shipperId, OrderStatus? orderStatus, OrderSortBy? sortCriteria,
                                                          bool descending, int pageIndex, int pageSize)
        {
            var list = await ApplyFilterToList(orderStatus, sortCriteria, descending, shipperId);
            var result = _mapper.Map<List<OrderListDTO>>(list);
            return await ListPagination<OrderListDTO>.PaginateList(result, pageIndex, pageSize);
        }



        public async Task<string> CreateOrder(OrderCreateDTO request)
        {
            int number = 0;
            var user = await _unitOfWork.OcopPartnerRepo.AnyAsync(u => u.Id == request.OcopPartnerId);
            if (user.Equals(false))
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            int firstProductWarehouseId = 0;
            int index = 0;
            decimal? totalPrice = 0;
            decimal? totalStorageFee = 0;
            decimal? deliveryFee = 0;
            foreach (var product in request.Products)
            {

                var a = await _unitOfWork.ProductRepo.SingleOrDefaultAsync(u => u.Id.Equals(product.ProductId));
                if (a == null)
                {
                    throw new KeyNotFoundException(ResponseMessage.ProductIdNotFound);
                }
                //get a list of Lot with product Id
                var b = await _unitOfWork.LotRepo.GetQueryable(query => query.Where(u => u.ProductId.Equals(product.ProductId)
                                                                && u.InventoryId.HasValue
                                                                && u.ProductAmount > 0
                                                                && u.IsDeleted.Equals(false))
                                                                .OrderBy(u => u.ImportDate)
                                                                .Include(o => o.Product)
                                                                .Include(o => o.Inventory).ThenInclude(o => o.Warehouse));
                if (b.Count() == 0)
                {
                    throw new KeyNotFoundException(ResponseMessage.NoLotWithProductFound);
                }

                //get first product warehouse Id
                if (firstProductWarehouseId == 0 && b.Any())
                {
                    firstProductWarehouseId = b[0].Inventory.WarehouseId.Value;
                }

                //if first product warehouse id already exist, check if this product is in the same warehouse or not
                if (firstProductWarehouseId != 0)
                {
                    if (b[0].Inventory.WarehouseId != firstProductWarehouseId)
                    {
                        for (int i = 1; i < b.Count(); i++)
                        {
                            if (b[i].Inventory.WarehouseId == firstProductWarehouseId)
                            {
                                index = i;
                                break;
                            }
                        }
                        if (index == 0)
                        {
                            throw new ApplicationException(ResponseMessage.ProductMustBeFromTheSameWarehouse);
                        }

                    }
                }

                //check for product amount

                //Get the next Lot if the first Lot doesnt have enough product
                //check if the next Lot have the same warehouseId or not, if not then go to the next Lot
                //if none of the next Lot have the same warehouseId return not enough product
                //if there is no next Lot return not enough product
                int productAmountNeeded = product.ProductAmount;

                while (productAmountNeeded > 0)
                {
                    if (index >= b.Count())
                    {
                        throw new ApplicationException(ResponseMessage.ProductNotEnough);
                    }

                    var lot = b[index];

                    // Check if the current lot's WarehouseId matches the first lot's WarehouseId
                    if (lot.Inventory.WarehouseId != firstProductWarehouseId)
                    {
                        // If not, move to the next lot and continue the loop
                        index++;
                        continue;
                    }

                    int amountToTake = 0;

                    if (lot.ProductAmount >= productAmountNeeded)
                    {
                        amountToTake = productAmountNeeded;
                        productAmountNeeded = 0;  // Product amount is fulfilled.
                    }
                    else
                    {
                        amountToTake = lot.ProductAmount.Value;
                        productAmountNeeded -= lot.ProductAmount.Value;
                    }

                    totalPrice += lot.Product.Price * amountToTake;

                    request.OrderDetails.Add(new OrderDetailCreateDTO
                    {
                        LotId = lot.Id,
                        ProductAmount = amountToTake,
                        ProductPrice = (int)(lot.Product.Price)
                    });

                    index++; // Move to the next lot
                }
                if (productAmountNeeded > 0)
                {
                    throw new ApplicationException(ResponseMessage.ProductNotEnough);
                }
                index = 0;
            }


            var result = _mapper.Map<Order>(request);
            result.CreateDate = DateTime.Now;
            result.Status = Constants.Status.Draft;
            result.TotalPrice = totalPrice;
            await _unitOfWork.OrderRepo.AddAsync(result);
            await _unitOfWork.SaveAsync();

            var orderFee = new OrderFee
            {
                OrderId = result.Id,
                AdditionalFee = 0,
                StorageFee = totalStorageFee,
                DeliveryFee = deliveryFee,
                IsDeleted = false
            };
            await _unitOfWork.OrderFeeRepo.AddAsync(orderFee);
            await _unitOfWork.SaveAsync();

            return ResponseMessage.Success;

        }

        public async Task<string> DeleteOrder(int id)
        {
            var exist = await _unitOfWork.OrderRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.OrderIdNotFound);
            }
            if (exist.Status != Constants.Status.Draft)
            {
                throw new ApplicationException(ResponseMessage.OrderProccessedError);
            }
            _unitOfWork.OrderRepo.SoftDelete(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> UpdateOrder(int id, OrderUpdateDTO request)
        {
            var exist = await _unitOfWork.OrderRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.OrderIdNotFound);
            }
            var firstODs = await _unitOfWork.LotRepo.SingleOrDefaultAsync(u => u.Id.Equals(request.OrderDetails.First().LotId), query => query.Include(o => o.Inventory));
            if (firstODs == null)
            {
                throw new KeyNotFoundException(ResponseMessage.PackageIdNotFound);
            }

            if (exist.Status != Constants.Status.Draft && exist.Status != Constants.Status.Shipping)
            {
                if (request.OrderDetails != null)
                {
                    throw new ApplicationException(ResponseMessage.OrderProccessedError);
                }
            }
            //If status = draft update orderdetails and such
            if (exist.Status == Constants.Status.Draft)
            {
                decimal? totalPrice = 0;
                foreach (var x in exist.OrderDetails)
                {
                    _unitOfWork.OrderDetailRepo.HardDelete(x);
                }

                foreach (var x in request.OrderDetails)
                {
                    var a = await _unitOfWork.LotRepo.SingleOrDefaultAsync(u => u.Id.Equals(x.LotId), query => query.Include(o => o.Product).Include(o => o.Inventory));
                    if (a == null)
                    {
                        throw new KeyNotFoundException(ResponseMessage.PackageIdNotFound);
                    }

                    totalPrice += a.Product.Price * x.ProductAmount;
                    x.ProductPrice = (int)(a.Product.Price);
                    if (a.Inventory.WarehouseId != firstODs.Inventory.WarehouseId)
                    {
                        throw new ApplicationException(ResponseMessage.OrderDetailsError);
                    }
                }
                exist.TotalPrice = totalPrice;
                exist.OrderDetails = _mapper.Map<List<OrderDetail>>(request);
            }


            foreach (var fee in exist.OrderFees)
            {
                //if status = shipping ad additional fee
                if (exist.Status == Constants.Status.Shipping)
                {
                    fee.AdditionalFee = 1;
                }
                //update delivery fee and storage fee in both situation
                fee.DeliveryFee = 1;
                fee.StorageFee = 1;
            }

            //update receiver address and phone in both draft and shipping
            exist.ReceiverAddress = request.ReceiverAddress;
            exist.ReceiverPhone = request.ReceiverPhone;

            _unitOfWork.OrderRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        //Partner use this and the one below
        public async Task<string> SendOrder(int id)
        {
            var exist = await _unitOfWork.OrderRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.OrderIdNotFound);
            }
            if (exist.Status != Constants.Status.Draft)
            {
                throw new ApplicationException(ResponseMessage.OrderSentError);
            }
            exist.Status = Constants.Status.Pending;
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> CancelOrder(int id)
        {
            var exist = await _unitOfWork.OrderRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.OrderIdNotFound);
            }
            if (exist.Status == Constants.Status.Pending ||
                exist.Status == Constants.Status.Shipping)
            {
                exist.Status = Constants.Status.Canceled;
                await _unitOfWork.SaveAsync();
            }
            else
            {
                throw new ApplicationException(ResponseMessage.OrderCanceledError);
            }
            return ResponseMessage.Success;
        }


        //Shipper and staff use this
        public async Task<string> UpdateOrderStatus(int id, OrderStatus orderStatus)
        {
            var exist = await _unitOfWork.OrderRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.OrderIdNotFound);
            }

            var orderStatusString = orderStatus.ToString();
            bool a = false;
            string orderStatusUpdate = null;
            if (orderStatusString.Equals(Constants.Status.Pending, StringComparison.OrdinalIgnoreCase))        //Pending
            {
                return ResponseMessage.Success;
            }

            //from Pending to Processing
            if (orderStatusString.Equals(Constants.Status.Processing, StringComparison.OrdinalIgnoreCase))        //Processing
            {
                if (exist.Status == Constants.Status.Pending)
                {
                    orderStatusUpdate = Constants.Status.Processing;
                    a = true;
                }
                else
                {
                    throw new ApplicationException(ResponseMessage.OrderProccessedError);
                }
            }

            //from processing to Shipping
            if (orderStatusString.Equals(Constants.Status.Shipping, StringComparison.OrdinalIgnoreCase))    //Shipped
            {
                if (exist.Status == Constants.Status.Processing)
                {
                    orderStatusUpdate = Constants.Status.Shipping;
                    a = true;
                    //take away the product's amount here
                    foreach (var od in exist.OrderDetails)
                    {
                        UpdateLotProductAmount(od.LotId, od.ProductAmount, false);
                    }
                }
                else
                {
                    throw new ApplicationException(ResponseMessage.OrderProccessedError);
                }
            }

            //Order can be canceled from three states,
            //Pending (Partner initiate),
            //Proccessing (Both User and Staff can initiate), 
            //Shipping (Partner initiate) (additional fee)
            //the reason why there is no "Pending" below is because I want to seperate Partner cancel from this
            if (orderStatusString.Equals(Constants.Status.Canceled, StringComparison.OrdinalIgnoreCase)) //Canceled
            {
                if (exist.Status == Constants.Status.Shipping ||
                    exist.Status == Constants.Status.Processing)
                {
                    orderStatusUpdate = Constants.Status.Canceled;
                    a = true;
                    //return product's amount here
                    foreach (var od in exist.OrderDetails)
                    {
                        UpdateLotProductAmount(od.LotId, od.ProductAmount, true);
                    }
                }
                else
                {
                    throw new ApplicationException(ResponseMessage.OrderCanceledError);
                }
            }

            //From Shipping to delivered
            if (orderStatusString.Equals(Constants.Status.Delivered, StringComparison.OrdinalIgnoreCase))
            {
                if (exist.Status == Constants.Status.Shipping)
                {
                    orderStatusUpdate = Constants.Status.Delivered;
                    a = true;
                }
                else
                {
                    throw new ApplicationException();
                }
            }

            if (!a)
            {
                throw new BadHttpRequestException(ResponseMessage.BadRequest);
            }

            exist.Status = orderStatusUpdate;
            _unitOfWork.OrderRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }



        private async Task UpdateLotProductAmount(int? lotId, int? amount, bool cancel)
        {
            var lot = await _unitOfWork.LotRepo.SingleOrDefaultAsync(u => u.Id == lotId);
            if (lot != null)
            {
                throw new KeyNotFoundException(ResponseMessage.PackageIdNotFound);
            }

            if (cancel == true)
            {
                lot.ProductAmount += amount;
            }
            else
            {
                lot.ProductAmount -= amount;
            }
            await _unitOfWork.SaveAsync();
        }
    }
}
