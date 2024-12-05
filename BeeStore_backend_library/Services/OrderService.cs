using Amazon.S3.Model;
using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.OrderDTOs;
using BeeStore_Repository.Enums;
using BeeStore_Repository.Enums.FilterBy;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using MySqlX.XDevAPI.Common;
using System.Data;
using System.Linq.Expressions;

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

        private async Task<List<Order>> ApplyFilterToList(bool? hasBatch, OrderFilterBy? orderFilterBy, string? filterQuery, OrderStatus? orderStatus, OrderSortBy? sortCriteria,
                                                          bool descending, int? shipperId = null, int? userId = null, int? warehouseId = null)
        {
            string? filterQue = orderStatus switch
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

            //Expression<Func<Order, bool>> filterExpression = null;
            //switch (orderFilterBy)
            //{
            //    case OrderFilterBy.DeliveryZoneId:
            //        break;
            //    case OrderFilterBy.BatchId: 
            //        if (filterQuery.Equals("false", StringComparison.OrdinalIgnoreCase)){
            //            filterExpression = u => u.BatchId == null;
            //        }
            //        else
            //        {
            //            filterExpression = u => u.BatchId != null;
            //        }
            //            break;
            //    case null: filterQuery = string.Empty; break;
            //}

            

            string? sortBy = sortCriteria switch
            {
                OrderSortBy.CreateDate => Constants.SortCriteria.CreateDate,
                OrderSortBy.TotalPrice => Constants.SortCriteria.TotalPrice,
                OrderSortBy.ProductAmount => Constants.SortCriteria.ProductAmount,
                _ => null
            };

            Expression<Func<Order, bool>> combinedFilter = u => (filterQue == null || u.Status.Equals(filterQue))
                             && (userId == null || u.OcopPartnerId.Equals(userId))
                             && (warehouseId == null || u.OrderDetails.Any(od => od.Lot.Inventory.WarehouseId.Equals(warehouseId)))
                             && (shipperId == null || u.Batch.DeliverBy.Equals(shipperId) && u.IsDeleted.Equals(false))
                             && (orderFilterBy == null || u.DeliveryZoneId.Equals(Int32.Parse(filterQuery)))
                             && (hasBatch == null ||
        (hasBatch == true && u.BatchId != null) ||
        (hasBatch == false && u.BatchId == null));


            var list = await _unitOfWork.OrderRepo.GetListAsync(
                filter: combinedFilter,
                includes: u => u.Include(o => o.Batch),
                sortBy: sortBy!,
                descending: descending,
                searchTerm: null,
                searchProperties: null
                );

           
            return list;
        }

        public async Task<Pagination<OrderListDTO>> GetOrderList(bool? hasBatch,OrderFilterBy? orderFilterBy, string? filterQuery, OrderStatus? orderStatus, OrderSortBy? sortCriteria,
                                                          bool descending, int pageIndex, int pageSize)
        {
            var list = await ApplyFilterToList(hasBatch, orderFilterBy, filterQuery, orderStatus, sortCriteria, descending);
            var result = _mapper.Map<List<OrderListDTO>>(list);
            return await ListPagination<OrderListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<Pagination<OrderListDTO>> GetWarehouseSentOrderList(bool? hasBatch, OrderFilterBy? orderFilterBy, string? filterQuery, int warehouseId, OrderStatus? orderStatus, OrderSortBy? sortCriteria, bool descending, int pageIndex, int pageSize)
        {
            var list = await ApplyFilterToList(hasBatch, orderFilterBy, filterQuery, orderStatus, sortCriteria, descending, null, null, warehouseId);
            var result = _mapper.Map<List<OrderListDTO>>(list);
            return await ListPagination<OrderListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<Pagination<OrderListDTO>> GetOrderList(bool? hasBatch, OrderFilterBy? orderFilterBy, string? filterQuery, int userId, OrderStatus? orderStatus, OrderSortBy? sortCriteria,
                                                          bool descending, int pageIndex, int pageSize)
        {
            var list = await ApplyFilterToList(hasBatch, orderFilterBy, filterQuery, orderStatus, sortCriteria, descending, null, userId);
            var result = _mapper.Map<List<OrderListDTO>>(list);
            return await ListPagination<OrderListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<Pagination<OrderListDTO>> GetDeliverOrderList(bool? hasBatch, OrderFilterBy? orderFilterBy, string? filterQuery, int shipperId, OrderStatus? orderStatus, OrderSortBy? sortCriteria,
                                                          bool descending, int pageIndex, int pageSize)
        {
            var list = await ApplyFilterToList(hasBatch, orderFilterBy, filterQuery, orderStatus, sortCriteria, descending, shipperId);
            var result = _mapper.Map<List<OrderListDTO>>(list);
            return await ListPagination<OrderListDTO>.PaginateList(result, pageIndex, pageSize);
        }



        public async Task<string> CreateOrder(int warehouseId, OrderCreateDTO request)
        {
            int number = 0;
            var user = await _unitOfWork.OcopPartnerRepo.AnyAsync(u => u.Id == request.OcopPartnerId);
            if (user.Equals(false))
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            //int firstProductWarehouseId = 0;
            int index = 0;
            decimal? totalPrice = 0;
            decimal? totalStorageFee = 0;
            decimal? deliveryFee = 0;
            decimal? totalWeight = 0;
            if(request.Distance > 10)
            {
                deliveryFee += (request.Distance - 10) * 1000;
            }
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
                                                                && u.ImportDate.HasValue
                                                                && u.TotalProductAmount > 0
                                                                && u.Inventory.WarehouseId.Equals(warehouseId)
                                                                && u.IsDeleted.Equals(false))
                                                                .OrderBy(u => u.ImportDate)
                                                                .Include(o => o.Product)
                                                                .Include(o => o.Inventory).ThenInclude(o => o.Warehouse));
                if (b.Count() == 0)
                {
                    throw new KeyNotFoundException(ResponseMessage.NoLotWithProductFound);
                }

                //get first product warehouse Id
                //if (firstProductWarehouseId == 0 && b.Any())
                //{
                //    firstProductWarehouseId = b[0].Inventory.WarehouseId.Value;
                //}

                ////if first product warehouse id already exist, check if this product is in the same warehouse or not
                //if (firstProductWarehouseId != 0)
                //{
                //    if (b[0].Inventory.WarehouseId != firstProductWarehouseId)
                //    {
                //        for (int i = 1; i < b.Count(); i++)
                //        {
                //            if (b[i].Inventory.WarehouseId == firstProductWarehouseId)
                //            {
                //                index = i;
                //                break;
                //            }
                //        }
                //        if (index == 0)
                //        {
                //            throw new ApplicationException(ResponseMessage.ProductMustBeFromTheSameWarehouse);
                //        }

                //    }
                //}

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
                    //if (lot.Inventory.WarehouseId != firstProductWarehouseId)
                    //{
                    //    // If not, move to the next lot and continue the loop
                    //    index++;
                    //    continue;
                    //}

                    int amountToTake = 0;

                    if (lot.TotalProductAmount >= productAmountNeeded)
                    {
                        amountToTake = productAmountNeeded;
                        productAmountNeeded = 0;  // Product amount is fulfilled.
                    }
                    else
                    {
                        amountToTake = lot.TotalProductAmount.Value;
                        productAmountNeeded -= lot.TotalProductAmount.Value;
                    }

                    totalPrice += lot.Product.Price * amountToTake;
                    totalStorageFee += CalculateStorageFee(lot.ImportDate.Value, DateTime.Now);
                    totalWeight += lot.Product.Weight * product.ProductAmount;
                    if(lot.Product.Weight * product.ProductAmount > 5)
                    {
                        decimal? extraWeight = lot.Product.Weight * product.ProductAmount - 5;
                        decimal extraWeightUnits = Math.Ceiling((decimal)extraWeight / 0.5m); // Convert to 0.5kg units
                        deliveryFee += extraWeightUnits * 5000;
                    }
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
            result.TotalWeight = totalWeight;
            result.TotalPriceAfterFee = totalPrice - (totalStorageFee + deliveryFee);
            result.OrderCode = GenerateOrderCode();
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
            foreach(var x in exist.OrderDetails)
            {
                _unitOfWork.OrderDetailRepo.SoftDelete(x);
            }
            foreach(var x in exist.OrderFees)
            {
                _unitOfWork.OrderFeeRepo.SoftDelete(x);
            }
            _unitOfWork.OrderRepo.SoftDelete(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> UpdateOrder(int id, int warehouseId, OrderUpdateDTO request)
        {
            var exist = await _unitOfWork.OrderRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.OrderIdNotFound);
            }

            if (exist.Status != Constants.Status.Draft && exist.Status != Constants.Status.Shipping)
            {
                int additionalFeeAfterUpdate = 5000;

                //if Draft update normally
                if (exist.Status == Constants.Status.Draft)
                {
                    //delete existing order details and fees
                    foreach (var x in exist.OrderDetails)
                    {
                        _unitOfWork.OrderDetailRepo.HardDelete(x);
                    }
                    foreach(var x in exist.OrderFees)
                    {
                        _unitOfWork.OrderFeeRepo.HardDelete(x);
                    }

                    //copy from create order
                    int index = 0;
                    decimal? totalPrice = 0;
                    decimal? totalStorageFee = 0;
                    decimal? deliveryFee = 0;
                    decimal? totalWeight = 0;
                    if (request.Distance > 10)
                    {
                        deliveryFee += (request.Distance - 10) * 1000;
                    }
                    foreach (var product in request.Products)
                    {

                        var a = await _unitOfWork.ProductRepo.AnyAsync(u => u.Id.Equals(product.ProductId));
                        if (a == false)
                        {
                            throw new KeyNotFoundException(ResponseMessage.ProductIdNotFound);
                        }

                        var b = await _unitOfWork.LotRepo.GetQueryable(query => query.Where(u => u.ProductId.Equals(product.ProductId)
                                                                        && u.InventoryId.HasValue
                                                                        && u.ImportDate.HasValue
                                                                        && u.TotalProductAmount > 0
                                                                        && u.Inventory.WarehouseId.Equals(warehouseId)
                                                                        && u.IsDeleted.Equals(false))
                                                                        .OrderBy(u => u.ImportDate)
                                                                        .Include(o => o.Product)
                                                                        .Include(o => o.Inventory).ThenInclude(o => o.Warehouse));
                        if (b.Count() == 0)
                        {
                            throw new KeyNotFoundException(ResponseMessage.NoLotWithProductFound);
                        }


                        int productAmountNeeded = product.ProductAmount;

                        while (productAmountNeeded > 0)
                        {
                            if (index >= b.Count())
                            {
                                throw new ApplicationException(ResponseMessage.ProductNotEnough);
                            }

                            var lot = b[index];

                            int amountToTake = 0;

                            if (lot.TotalProductAmount >= productAmountNeeded)
                            {
                                amountToTake = productAmountNeeded;
                                productAmountNeeded = 0;  // Product amount is fulfilled.
                            }
                            else
                            {
                                amountToTake = lot.TotalProductAmount.Value;
                                productAmountNeeded -= lot.TotalProductAmount.Value;
                            }

                            totalPrice += lot.Product.Price * amountToTake;
                            totalStorageFee += CalculateStorageFee(lot.ImportDate.Value, DateTime.Now);
                            totalWeight += lot.Product.Weight * product.ProductAmount;
                            if (lot.Product.Weight * product.ProductAmount > 5)
                            {
                                decimal? extraWeight = lot.Product.Weight * product.ProductAmount - 5;
                                decimal extraWeightUnits = Math.Ceiling((decimal)extraWeight / 0.5m); // Convert to 0.5kg units
                                deliveryFee += extraWeightUnits * 5000;
                            }

                            exist.OrderDetails.Add(new OrderDetail
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
                    exist.OrderFees.Add(new OrderFee
                    {
                        AdditionalFee = additionalFeeAfterUpdate,
                        StorageFee = totalStorageFee,
                        DeliveryFee = deliveryFee,
                        IsDeleted = false
                    });
                    exist.Distance = request.Distance;
                    exist.TotalPrice = totalPrice;
                    exist.TotalWeight = totalWeight;
                    exist.TotalPriceAfterFee = totalPrice - (totalStorageFee + deliveryFee + additionalFeeAfterUpdate);
                }

                //if shipping only allow update for receiver address and phone.
                if(exist.Status == Constants.Status.Shipping)
                {
                    if (request.OrderDetails != null)
                    {
                        throw new ApplicationException(ResponseMessage.OrderProccessedError);
                    }

                    //if status = shipping ad additional fee
                    if (exist.Status == Constants.Status.Shipping)
                    {
                        exist.OrderFees.First(u => u.OrderId.Equals(exist.Id)).AdditionalFee = additionalFeeAfterUpdate;
                    }
                    exist.TotalPriceAfterFee -= additionalFeeAfterUpdate;

                }
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
                exist.CancelDate = DateTime.Now;
                await _unitOfWork.SaveAsync();
            }
            else
            {
                throw new ApplicationException(ResponseMessage.OrderCanceledError);
            }
            return ResponseMessage.Success;
        }


        //Shipper and staff use this
        public async Task<string> UpdateOrderStatus(int id, OrderStatus orderStatus, string? cancellationReason)
        {
            var exist = await _unitOfWork.OrderRepo.SingleOrDefaultAsync(u => u.Id == id,
                                                                        query => query.Include(o => o.Batch));
                                                                        
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
                    //take away the product's amount here
                    foreach (var od in exist.OrderDetails)
                    {
                        await UpdateLotProductAmount(od.LotId, od.ProductAmount, false);
                    }
                    exist.ApproveDate = DateTime.Now;
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
                    
                }
                else
                {
                    throw new ApplicationException(ResponseMessage.OrderProccessedError);
                }
            }

            if (orderStatusString.Equals(Constants.Status.Returned, StringComparison.OrdinalIgnoreCase))    //Returned
            {
                if (exist.Status == Constants.Status.Delivered)
                {
                    orderStatusUpdate = Constants.Status.Returned;
                    exist.ReturnDate = DateTime.Now;
                    a = true;
                    exist.CancellationReason = cancellationReason;
                    //take away the product's amount here
                    foreach (var od in exist.OrderDetails)
                    {
                        await UpdateLotProductAmount(od.LotId, od.ProductAmount, true);
                    }
                }
                else
                {
                    throw new ApplicationException(ResponseMessage.OrderProccessedError);
                }
            }

            
            if (orderStatusString.Equals(Constants.Status.Canceled, StringComparison.OrdinalIgnoreCase)) //Canceled
            {
                if (exist.Status == Constants.Status.Shipping ||
                    exist.Status == Constants.Status.Processing)
                {
                    orderStatusUpdate = Constants.Status.Canceled;
                    a = true;
                    exist.CancelDate = DateTime.Now;
                    exist.CancellationReason = cancellationReason;
                    //return product's amount here
                    foreach (var od in exist.OrderDetails)
                    {
                        await UpdateLotProductAmount(od.LotId, od.ProductAmount, true);
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
                    var orderfee = exist.OrderFees.FirstOrDefault(u => u.Id.Equals(exist.Id));
                    exist.Payments.Add(new Payment
                    {
                        OcopPartnerId = exist.OcopPartnerId,
                        CollectedBy = exist.Batch.DeliverBy,
                        OrderId = exist.Id,
                        TotalAmount = exist.TotalPriceAfterFee
                    //    TotalAmount = (int)(exist.TotalPrice - (orderfee.DeliveryFee + orderfee.StorageFee + orderfee.AdditionalFee))
                    });
                    exist.DeliverFinishDate = DateTime.Now;
                    //add money directly after order is delivered.
                    exist.OcopPartner.Wallets.FirstOrDefault(u => u.Id.Equals(exist.OcopPartnerId)).TotalAmount += exist.TotalPriceAfterFee;
                }
                else
                {
                    throw new ApplicationException();
                }
            }

            if(orderStatusString.Equals(Constants.Status.Refunded, StringComparison.OrdinalIgnoreCase))
            {
                if(exist.Status == Constants.Status.Returned)
                {
                    orderStatusUpdate = Constants.Status.Refunded; 
                    a = true;
                }
                else
                {
                    throw new ApplicationException();
                }
            }

            if(orderStatusString.Equals(Constants.Status.Completed, StringComparison.OrdinalIgnoreCase))
            {
                if(exist.Status == Constants.Status.Delivered)
                {
                    orderStatusUpdate = Constants.Status.Completed;
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

        private decimal CalculateStorageFee(DateTime importDate, DateTime currentDate)
        {
            if(importDate.Date == currentDate.Date)
            {
                return 0;
            }

            const decimal RATE_FIRST_WEEK = 10;    
            const decimal RATE_AFTER_WEEK = 20;    
            const int DAYS_IN_WEEK = 7;

            TimeSpan storageDuration = currentDate - importDate;

            double totalHours = storageDuration.TotalHours;

            double hoursInFirstWeek = Math.Min(totalHours, DAYS_IN_WEEK * 24);
            double hoursAfterFirstWeek = Math.Max(0, totalHours - (DAYS_IN_WEEK * 24));

            decimal firstWeekFee = (decimal)hoursInFirstWeek * RATE_FIRST_WEEK;
            decimal remainingFee = (decimal)hoursAfterFirstWeek * RATE_AFTER_WEEK;

            return firstWeekFee + remainingFee;
        }


        private async Task UpdateLotProductAmount(int? lotId, int? amount, bool cancel)
        {
            var lot = await _unitOfWork.LotRepo.SingleOrDefaultAsync(u => u.Id == lotId);
            if (lot == null)
            {
                throw new KeyNotFoundException(ResponseMessage.PackageIdNotFound);
            }

            if (cancel == true)
            {
                lot.TotalProductAmount += amount;
                lot.Inventory.Weight += amount * lot.Product.Weight;
            }
            else
            {
                if (lot.TotalProductAmount == 0 || lot.TotalProductAmount < amount)
                {
                    throw new ApplicationException(ResponseMessage.ProductNotEnough);
                }
                lot.TotalProductAmount -= amount;
                lot.Inventory.Weight -= amount * lot.Product.Weight;
            }
            await _unitOfWork.SaveAsync();
        }

        private string GenerateOrderCode()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string randomSuffix = new Random().Next(1000, 9999).ToString();
            return $"ORD-{timestamp}-{randomSuffix}";
        }
    }
}
