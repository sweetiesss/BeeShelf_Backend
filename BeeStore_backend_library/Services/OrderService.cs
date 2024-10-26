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
using System.Linq.Expressions;
using static BeeStore_Repository.Utils.Constants;

namespace BeeStore_Repository.Services
{
    public class OrderService : IOrderService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public OrderService(UnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
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
                             && (userId == null || u.OcopPartnerId.Equals(userId)),
                             //&& (shipperId == null || u.Equals(shipperId)),
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

        //this shit sucked, will have to think about later
        public async Task<Pagination<OrderListDTO>> GetWarehouseSentOrderList(int warehouseId, OrderStatus? orderStatus, OrderSortBy? sortCriteria, bool descending, int pageIndex, int pageSize)
        {
            var list = await ApplyFilterToList(orderStatus, sortCriteria, descending, null, null, warehouseId);
            var result = _mapper.Map<List<OrderListDTO>>(list);
            return await ListPagination<OrderListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<Pagination<OrderListDTO>> GetOrderList(int userId, OrderStatus? orderStatus, OrderSortBy? sortCriteria,
                                                          bool descending, int pageIndex, int pageSize)
        {
            var list = await ApplyFilterToList(orderStatus, sortCriteria, descending, null ,userId);
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
            var user = await _unitOfWork.OcopPartnerRepo.AnyAsync(u => u.Id == request.OcopPartnerId);
            if (user.Equals(false))
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }

            int? totalPrice = 0;
            foreach(var od in request.OrderDetails)
            {
                totalPrice += od.ProductPrice;
                var a = await _unitOfWork.LotRepo.AnyAsync(u => u.Id.Equals(od.LotId));
                if (a.Equals(false))
                {
                    throw new KeyNotFoundException(ResponseMessage.PackageIdNotFound);
                }
            }

            

            request.CreateDate = DateTime.Now;
            request.Status = Constants.Status.Draft;
            request.TotalPrice = totalPrice;
            var result = _mapper.Map<Order>(request);
            await _unitOfWork.OrderRepo.AddAsync(result);
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
            if (exist.Status != Constants.Status.Pending)
            {
                throw new ApplicationException(ResponseMessage.OrderProccessedError);
            }
            _unitOfWork.OrderRepo.SoftDelete(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        //don't touch update order, im too lazy to fix it, might be later, for now just want to do create and getlist
        public async Task<string> UpdateOrder(int id, OrderCreateDTO request)
        {
            var exist = await _unitOfWork.OrderRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.OrderIdNotFound);
            }
            if (exist.Status != Constants.Status.Pending)
            {
                throw new ApplicationException(ResponseMessage.OrderProccessedError);
            }
            //exist.PictureId = request.PictureId;
            //exist.TotalPrice = request.TotalPrice;
            //exist.DeliverBy = request.DeliverBy;
            //exist.ReceiverPhone = request.ReceiverPhone;
            //exist.ReceiverAddress = request.ReceiverAddress;
            //exist.CodStatus = request.CodStatus;
            //exist.CancellationReason = request.CancellationReason;
            //exist.ProductAmount = request.ProductAmount;
            //exist.ProductId = request.ProductId;
            _unitOfWork.OrderRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        //Partner use this and the one below
        public async Task<string> SendOrder(int id)
        {
            var exist = await _unitOfWork.OrderRepo.SingleOrDefaultAsync(u => u.Id==id);
            if(exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.OrderIdNotFound);
            }
            if(exist.Status != Constants.Status.Draft)
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
                }
                else
                {
                    throw new ApplicationException(ResponseMessage.OrderCanceledError);
                }
            }

            //From Shipping to delivered
            if(orderStatusString.Equals(Constants.Status.Delivered, StringComparison.OrdinalIgnoreCase))
            {
                if(exist.Status == Constants.Status.Shipping)
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

  
    }
}
