﻿using AutoMapper;
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
using System.Data;
using System.Linq.Expressions;
using static BeeStore_Repository.Utils.Constants;

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
                                                          bool descending, int? shipperId = null, int? userId = null, int? storeId = null)
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

            string? sortBy = sortCriteria switch
            {
                OrderSortBy.CreateDate => Constants.SortCriteria.CreateDate,
                OrderSortBy.TotalPrice => Constants.SortCriteria.TotalPrice,
                OrderSortBy.ProductAmount => Constants.SortCriteria.ProductAmount,
                _ => null
            };

            Expression<Func<Order, bool>> combinedFilter = u => (filterQue == null || u.Status.Equals(filterQue))
                             && (userId == null || u.OcopPartnerId.Equals(userId))
                             && (storeId == null || u.OrderDetails.Any(od => od.Lot.Room.StoreId.Equals(storeId)))
                             && (shipperId == null || u.Batch.DeliverBy.Equals(shipperId) && u.IsDeleted.Equals(false))
                             && (orderFilterBy == null || u.DeliveryZoneId.Equals(Int32.Parse(filterQuery)))
                             && (hasBatch == null ||
        (hasBatch == true && u.BatchId != null) ||
        (hasBatch == false && u.BatchId == null));


            var list = await _unitOfWork.OrderRepo.GetListAsync(
                filter: combinedFilter,
                includes: u => u.Include(o => o.Batch)
                                .Include(o => o.OrderDetails)
                                    .ThenInclude(o => o.Lot)
                                    .ThenInclude(o => o.Product)
                                .Include(o => o.OrderDetails)
                                    .ThenInclude(o => o.Lot)
                                    .ThenInclude(o => o.Room)
                                    .ThenInclude(o => o.Store)
                                .Include(o => o.OrderFees)
                                .Include(o => o.OcopPartner)
                                .Include(o => o.DeliveryZone),
                sortBy: sortBy!,
                descending: descending,
                searchTerm: null,
                searchProperties: null
                );


            return list;
        }

        public async Task<Pagination<OrderListDTO>> GetOrderList(bool? hasBatch, OrderFilterBy? orderFilterBy, string? filterQuery, OrderStatus? orderStatus, OrderSortBy? sortCriteria,
                                                          bool descending, int pageIndex, int pageSize)
        {
            var list = await ApplyFilterToList(hasBatch, orderFilterBy, filterQuery, orderStatus, sortCriteria, descending);
            var result = _mapper.Map<List<OrderListDTO>>(list);
            return await ListPagination<OrderListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<Pagination<OrderListDTO>> GetStoreSentOrderList(bool? hasBatch, OrderFilterBy? orderFilterBy, string? filterQuery, int storeId, OrderStatus? orderStatus, OrderSortBy? sortCriteria, bool descending, int pageIndex, int pageSize)
        {
            var list = await ApplyFilterToList(hasBatch, orderFilterBy, filterQuery, orderStatus, sortCriteria, descending, null, null, storeId);
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

        public async Task CreateOrderHandler(bool send, OrderCreateDTO request)
        {
            //for you who might dont understand wtf im doing(this could be me in the future)
            //Here's a brief description of main point

            //iterate through each product of the request
            foreach(var singleProductDetail in request.Products){

                var product = await _unitOfWork.ProductRepo.SingleOrDefaultAsync(u => u.Id.Equals(singleProductDetail.ProductId));
                if(product == null) throw new KeyNotFoundException(ResponseMessage.ProductIdNotFound);

                var TotalAmountOfProductInOrder = singleProductDetail.ProductAmount; // amount of product in the order

                //iterate through all the store of the product that FE send
                foreach(var ProductStore in singleProductDetail.ProductStore)
                {
                    var store = await _unitOfWork.StoreRepo.AnyAsync(u => u.Id.Equals(ProductStore.StoreId));
                    //get a list of Lot with productId (i copied you so hopefully it works)
                    var lotList = await _unitOfWork.LotRepo.GetQueryable(query => query.Where(u => u.ProductId.Equals(product.Id)
                                                                    && u.RoomId.HasValue
                                                                    && u.ImportDate.HasValue
                                                                    && u.TotalProductAmount > 0
                                                                    && u.Room.StoreId.Equals(ProductStore.StoreId)
                                                                    && u.IsDeleted.Equals(false))
                                                                    .OrderBy(u => u.ImportDate)
                                                                    .Include(o => o.Product)
                                                                    .Include(o => o.Room).ThenInclude(o => o.Store));
                    // determine how much should i take
                    int? totalNumberOfProduct = 0;
                    foreach(var lot in lotList){
                        totalNumberOfProduct += lot.TotalProductAmount;
                    }

                    // Create new Request to use the original CreateOrder - not gud
                    var newRequest = RewriteOrderCreateDTO(product.Id,
                                                           Math.Min(totalNumberOfProduct.Value, TotalAmountOfProductInOrder),
                                                           request);
                    
                    await CreateOrder(ProductStore.StoreId.Value, send, newRequest);

                    // Recheck if we have all of the product in order or not
                    TotalAmountOfProductInOrder -= totalNumberOfProduct.Value;
                    if (TotalAmountOfProductInOrder <= 0) break;
                }
            }
        }
        
        private OrderCreateDTO RewriteOrderCreateDTO(int productId, int productAmount, OrderCreateDTO request)
        {
            var productDetailDTO = new ProductDetailDTO();
            productDetailDTO.ProductId = productId;
            productDetailDTO.ProductAmount = productAmount;

            var newList = new List<ProductDetailDTO>()
            {
                productDetailDTO
            };

            var newOrderCreateDTO = new OrderCreateDTO()
            {
                OcopPartnerId = request.OcopPartnerId,
                DeliveryZoneId = request.DeliveryZoneId,
                Distance = request.Distance,
                ReceiverAddress = request.ReceiverAddress,
                ReceiverName = request.ReceiverName,
                ReceiverPhone = request.ReceiverPhone,
                Products = newList
            };


            return newOrderCreateDTO;
        }

        public async Task<string> CreateOrder(int storeId, bool send, OrderCreateDTO request)
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
            if (request.Distance > 10)
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
                                                                && u.RoomId.HasValue
                                                                && u.ImportDate.HasValue
                                                                && u.TotalProductAmount > 0
                                                                && u.Room.StoreId.Equals(storeId)
                                                                && u.IsDeleted.Equals(false))
                                                                .OrderBy(u => u.ImportDate)
                                                                .Include(o => o.Product)
                                                                .Include(o => o.Room).ThenInclude(o => o.Store));
                if (b.Count() == 0)
                {
                    throw new KeyNotFoundException(ResponseMessage.NoLotWithProductFound);
                }

                //gotta check for all the product of the same type here (all cold or non cold) here

                //

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
                        deliveryFee += extraWeightUnits * 2500;
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

            if (send == true)
            {
                result.Status = Constants.Status.Pending;
            }
            else
            {
                result.Status = Constants.Status.Draft;
            }

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
            foreach (var x in exist.OrderDetails)
            {
                _unitOfWork.OrderDetailRepo.SoftDelete(x);
            }
            foreach (var x in exist.OrderFees)
            {
                _unitOfWork.OrderFeeRepo.SoftDelete(x);
            }
            _unitOfWork.OrderRepo.SoftDelete(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> UpdateOrder(int id, int storeId, OrderUpdateDTO request)
        {
            var exist = await _unitOfWork.OrderRepo.SingleOrDefaultAsync(u => u.Id == id,
                                                                        includes => includes.Include(o => o.OrderFees).Include(o => o.OrderDetails));
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.OrderIdNotFound);
            }

            if (exist.Status == Constants.Status.Draft || exist.Status == Constants.Status.Shipping)
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
                    foreach (var x in exist.OrderFees)
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
                                                                        && u.RoomId.HasValue
                                                                        && u.ImportDate.HasValue
                                                                        && u.TotalProductAmount > 0
                                                                        && u.Room.StoreId.Equals(storeId)
                                                                        && u.IsDeleted.Equals(false))
                                                                        .OrderBy(u => u.ImportDate)
                                                                        .Include(o => o.Product)
                                                                        .Include(o => o.Room).ThenInclude(o => o.Store));
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
                                deliveryFee += extraWeightUnits * 2500;
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
                if (exist.Status == Constants.Status.Shipping)
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
            exist.ReceiverName = request.ReceiverName;
            exist.ReceiverAddress = request.ReceiverAddress;
            exist.ReceiverPhone = request.ReceiverPhone;

            //_unitOfWork.OrderRepo.Update(exist);
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
                                                                        query => query.Include(o => o.Batch)
                                                                                      .ThenInclude(o => o.Orders)
                                                                                      .Include(o => o.Batch)
                                                                                      .ThenInclude(o => o.DeliverByNavigation));

            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.OrderIdNotFound);
            }


            string? status = orderStatus switch
            {
                OrderStatus.Draft => string.Empty,
                OrderStatus.Pending => string.Empty,
                OrderStatus.Processing => await UpdateStatusProcessingOrder(orderStatus.ToString(), exist),
                OrderStatus.Shipping => await UpdateStatusShippingOrder(orderStatus.ToString(), exist),
                OrderStatus.Delivered => await UpdateStatusDeliveredOrder(orderStatus.ToString(), exist),
                OrderStatus.Completed => await UpdateStatusCompletedOrder(orderStatus.ToString(), exist),
                OrderStatus.Canceled => await UpdateStatusCanceledOrder(orderStatus.ToString(), exist, cancellationReason),
                OrderStatus.Returned => await UpdateStatusReturnedOrder(orderStatus.ToString(), exist, cancellationReason),
                OrderStatus.Refunded => await UpdateStatusRefundedOrder(orderStatus.ToString(), exist),
                _ => string.Empty
            };

            if(status == string.Empty)
            {
                throw new BadHttpRequestException(ResponseMessage.BadRequest);
            }
            //var orderStatusString = orderStatus.ToString();

            //from pending to processing
            //await UpdateStatusProcessingOrder(orderStatusString, exist);

            //from processing to shipping
            //await UpdateStatusShippingOrder(orderStatusString, exist);

            //From Shipping to delivered
            //await UpdateStatusDeliveredOrder(orderStatusString, exist);

            //From Delivered to Completed
            //await UpdateStatusCompletedOrder(orderStatusString, exist);

            //from shipping/processing to canceled
            //await UpdateStatusCanceledOrder(orderStatusString, exist, cancellationReason);

            //from delivered to returned
            //await UpdateStatusReturnedOrder(orderStatusString, exist, cancellationReason);

            //From Returned to refunded
            //await UpdateStatusRefundedOrder(orderStatusString, exist);

            return ResponseMessage.Success;
        }

        private async Task<string> UpdateStatusProcessingOrder(string orderStatus, Order exist)
        {
            //from Pending to Processing
            if (orderStatus.Equals(Constants.Status.Processing, StringComparison.OrdinalIgnoreCase))        //Processing
            {
                if (exist.Status == Constants.Status.Pending)
                {
                    orderStatus = Constants.Status.Processing;
                    //a = true;
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

            exist.Status = orderStatus;
            _unitOfWork.OrderRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        private async Task<string> UpdateStatusShippingOrder(string orderStatus, Order exist)
        {
            //from processing to Shipping
            if (orderStatus.Equals(Constants.Status.Shipping, StringComparison.OrdinalIgnoreCase))    //Shipped
            {
                if (exist.Status == Constants.Status.Processing)
                {
                    orderStatus = Constants.Status.Shipping;

                    // change the found vehicle's status to Available
                    exist.Batch.DeliverByNavigation.Vehicles.First().Status = Constants.VehicleStatus.InService;

                    //a = true;
                    DateTime now = DateTime.Now;
                    exist.DeliverStartDate = now;

                }
                else
                {
                    throw new ApplicationException(ResponseMessage.OrderProccessedError);
                }
            }
            exist.Status = orderStatus;
            _unitOfWork.OrderRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        private async Task<string> UpdateStatusDeliveredOrder(string orderStatus, Order exist)
        {
            //From Shipping to delivered
            if (orderStatus.Equals(Constants.Status.Delivered, StringComparison.OrdinalIgnoreCase))
            {
                if (exist.Status == Constants.Status.Shipping)
                {
                    orderStatus = Constants.Status.Delivered;
                    //a = true;
                    //create payment after delivered (but don't transfer money yet)
                    var orderfee = exist.OrderFees.FirstOrDefault(u => u.Id.Equals(exist.Id));
                    exist.Payments.Add(new Payment
                    {
                        OcopPartnerId = exist.OcopPartnerId,
                        CollectedBy = exist.Batch!.DeliverBy,
                        OrderId = exist.Id,
                        TotalAmount = exist.TotalPriceAfterFee
                    });
                    exist.DeliverFinishDate = DateTime.Now;
                }
                else
                {
                    throw new ApplicationException();
                }
            }
            exist.Status = orderStatus;
            _unitOfWork.OrderRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        private async Task<string> UpdateStatusCompletedOrder(string orderStatus, Order exist)
        {
            //From Delivered to Completed
            if (orderStatus.Equals(Constants.Status.Completed, StringComparison.OrdinalIgnoreCase)) // To Complete
            {
                if (exist.Status == Constants.Status.Delivered)
                {
                    bool b = true;
                    orderStatus = Constants.Status.Completed;


                    foreach (var x in exist.Batch.Orders)
                    {
                        if (x.Status == Constants.Status.Shipping || x.Status == Constants.Status.Delivered
                            || x.Status == Constants.Status.Returned || x.Status == Constants.Status.Processing)
                        {
                            if (x.Id != exist.Id)
                            {
                                b = false;
                                break;
                            }
                        }
                    }
                    if (b == true)
                    {
                        exist.Batch.Status = Constants.Status.Completed;
                    }
                    exist.Payments.First().IsConfirmed = 1;
                    exist.OcopPartner.Wallets.First().TotalAmount += exist.TotalPriceAfterFee;

                    // change the found vehicle's status to Available
                    exist.Batch.DeliverByNavigation.Vehicles.First().Status = Constants.VehicleStatus.Available;


                    //a = true;
                    exist.CompleteDate = DateTime.Now;
                }
                else
                {
                    throw new ApplicationException();
                }
            }
            exist.Status = orderStatus;
            _unitOfWork.OrderRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        private async Task<string> UpdateStatusCanceledOrder(string orderStatus, Order exist, string? cancellationReason)
        {
            //from shipping/processing to canceled
            if (orderStatus.Equals(Constants.Status.Canceled, StringComparison.OrdinalIgnoreCase)) //Canceled
            {
                bool b = true;
                if (exist.Status == Constants.Status.Shipping ||
                    exist.Status == Constants.Status.Processing)
                {
                    orderStatus = Constants.Status.Canceled;
                    //a = true;
                    exist.CancelDate = DateTime.Now;
                    exist.CancellationReason = cancellationReason;
                    //return product's amount here
                    foreach (var od in exist.OrderDetails)
                    {
                        await UpdateLotProductAmount(od.LotId, od.ProductAmount, true);
                    }
                    if (exist.Batch != null)
                    {
                        foreach (var x in exist.Batch.Orders)
                        {
                            if (x.Status == Constants.Status.Shipping || x.Status == Constants.Status.Delivered
                                || x.Status == Constants.Status.Returned || x.Status == Constants.Status.Processing)
                            {
                                if (x.Id != exist.Id)
                                {
                                    b = false;
                                    break;
                                }
                            }
                        }
                        if (b == true)
                        {
                            exist.Batch.Status = Constants.Status.Completed;
                        }

                        // change the found vehicle's status to Available
                        exist.Batch.DeliverByNavigation.Vehicles.First().Status = Constants.VehicleStatus.Available;
                    }
                }
                else
                {
                    throw new ApplicationException(ResponseMessage.OrderCanceledError);
                }
            }
            exist.Status = orderStatus;
            _unitOfWork.OrderRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }
        
        private async Task<string> UpdateStatusReturnedOrder(string orderStatus, Order exist, string? cancellationReason)
        {
            //from delivered to returned
            if (orderStatus.Equals(Constants.Status.Returned, StringComparison.OrdinalIgnoreCase))    //Returned
            {
                if (exist.Status == Constants.Status.Delivered)
                {
                    orderStatus = Constants.Status.Returned;
                    exist.ReturnDate = DateTime.Now;
                    //a = true;
                    exist.CancellationReason = cancellationReason;
                }
                else
                {
                    throw new ApplicationException(ResponseMessage.OrderProccessedError);
                }
            }
            exist.Status = orderStatus;
            _unitOfWork.OrderRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }
        
        private async Task<string> UpdateStatusRefundedOrder(string orderStatus, Order exist)
        {
            //From Returned to refunded
            if (orderStatus.Equals(Constants.Status.Refunded, StringComparison.OrdinalIgnoreCase)) // To refunded
            {
                if (exist.Status == Constants.Status.Returned)
                {
                    bool b = true;
                    orderStatus = Constants.Status.Refunded;

                    foreach (var od in exist.OrderDetails)
                    {
                        await UpdateLotProductAmount(od.LotId, od.ProductAmount, true);
                    }

                    if (exist.TotalPriceAfterFee < 0)
                    {
                        exist.TotalPriceAfterFee *= -1;
                    }
                    exist.OcopPartner.Wallets.First().TotalAmount -= exist.TotalPriceAfterFee;
                    exist.Payments.First().IsDeleted = true;

                    foreach (var x in exist.Batch.Orders)
                    {
                        if (x.Status == Constants.Status.Shipping || x.Status == Constants.Status.Delivered
                            || x.Status == Constants.Status.Returned || x.Status == Constants.Status.Processing)
                        {
                            if (x.Id != exist.Id)
                            {
                                b = false;
                                break;
                            }
                        }
                    }
                    if (b == true)
                    {
                        exist.Batch.Status = Constants.Status.Completed;
                    }

                    // change the found vehicle's status to Available
                    exist.Batch.DeliverByNavigation.Vehicles.First().Status = Constants.VehicleStatus.Available;

                    //a = true;
                }
                else
                {
                    throw new ApplicationException();
                }
            }
            exist.Status = orderStatus;
            _unitOfWork.OrderRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }
        
        private decimal CalculateStorageFee(DateTime importDate, DateTime currentDate)
        {
            currentDate = DateTime.UtcNow;
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime utcPlus7 = TimeZoneInfo.ConvertTimeFromUtc(currentDate, timeZone);

            if (importDate.Date == currentDate.Date)
            {
                return 0;
            }

            const decimal RATE_FIRST_WEEK = 10;
            const decimal RATE_AFTER_WEEK = 20;
            const int DAYS_IN_WEEK = 7;

            TimeSpan storageDuration = utcPlus7 - importDate;

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
                lot.Room.Weight += amount * lot.Product.Weight;
            }
            else
            {
                if (lot.TotalProductAmount == 0 || lot.TotalProductAmount < amount)
                {
                    throw new ApplicationException(ResponseMessage.ProductNotEnough);
                }
                lot.TotalProductAmount -= amount;
                lot.Room.Weight -= amount * lot.Product.Weight;
            }
            await _unitOfWork.SaveAsync();
        }

        private string GenerateOrderCode()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string randomSuffix = new Random().Next(1000, 9999).ToString();
            return $"ORD-{timestamp}-{randomSuffix}";
        }

        public async Task<OrderListDTO> GetOrder(int id)
        {
            var order = await _unitOfWork.OrderRepo.SingleOrDefaultAsync(u => u.Id.Equals(id),
                                u => u.Include(o => o.Batch)
                                .Include(o => o.OrderDetails)
                                    .ThenInclude(o => o.Lot)
                                    .ThenInclude(o => o.Product)
                                .Include(o => o.OrderDetails)
                                    .ThenInclude(o => o.Lot)
                                    .ThenInclude(o => o.Room)
                                    .ThenInclude(o => o.Store)
                                .Include(o => o.OrderFees)
                                .Include(o => o.OcopPartner)
                                .Include(o => o.DeliveryZone));
            if (order == null)
            {
                throw new KeyNotFoundException(ResponseMessage.OrderIdNotFound);
            }
            var result = _mapper.Map<OrderListDTO>(order);
            return result;
        }
    }
}
