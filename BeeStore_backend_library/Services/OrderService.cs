using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.OrderDTOs;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Pagination<OrderListDTO>> GetOrderList(int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.OrderRepo.GetAllAsync();
            var result = _mapper.Map<List<OrderListDTO>>(list);
            return await ListPagination<OrderListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<Pagination<OrderListDTO>> GetOrderList(int userId, int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.OrderRepo.GetFiltered(u => u.UserId.Equals(userId));
            var result = _mapper.Map<List<OrderListDTO>>(list);
            return await ListPagination<OrderListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<Pagination<OrderListDTO>> GetDeliverOrderList(int userId, int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.OrderRepo.GetFiltered(u => u.DeliverBy.Equals(userId));
            var result = _mapper.Map<List<OrderListDTO>>(list);
            return await ListPagination<OrderListDTO>.PaginateList(result, pageIndex, pageSize);
        }



        public async Task<string> CreateOrder(OrderCreateDTO request)
        {
            var user = await _unitOfWork.UserRepo.SingleOrDefaultAsync(u => u.Id == request.UserId,
                                                                       query => query.Include(o => o.Role));
            if (user == null)
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }

            if (user.RoleId != 4)
            {
                throw new ApplicationException(ResponseMessage.UserRoleNotPartnerError);
            }

            if (request.DeliverBy != 0)
            {
                var shipper = await _unitOfWork.UserRepo.SingleOrDefaultAsync(u => u.Id == request.DeliverBy);

                if (shipper == null)
                {
                    throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
                }
                if (shipper.RoleId != 5)
                {
                    throw new ApplicationException(ResponseMessage.UserRoleNotShipperError);
                }
            }

            var product = await _unitOfWork.ProductRepo.SingleOrDefaultAsync(u => u.Id == request.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException(ResponseMessage.ProductIdNotFound);
            }

            request.CreateDate = DateTime.Now;
            request.OrderStatus = Constants.Status.Pending;
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
            if (exist.OrderStatus != Constants.Status.Pending)
            {
                throw new ApplicationException(ResponseMessage.OrderProccessedError);
            }
            _unitOfWork.OrderRepo.SoftDelete(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> UpdateOrder(int id, OrderCreateDTO request)
        {
            var exist = await _unitOfWork.OrderRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.OrderIdNotFound);
            }
            if (exist.OrderStatus != Constants.Status.Pending)
            {
                throw new ApplicationException(ResponseMessage.OrderProccessedError);
            }
            exist.PictureId = request.PictureId;
            exist.TotalPrice = request.TotalPrice;
            exist.DeliverBy = request.DeliverBy;
            exist.ReceiverPhone = request.ReceiverPhone;
            exist.ReceiverAddress = request.ReceiverAddress;
            exist.CodStatus = request.CodStatus;
            exist.CancellationReason = request.CancellationReason;
            exist.ProductAmount = request.ProductAmount;
            exist.ProductId = request.ProductId;
            _unitOfWork.OrderRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> UpdateOrderStatus(int id, int orderStatus)
        {
            var exist = await _unitOfWork.OrderRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.OrderIdNotFound);
            }
            string orderStatusUpdate = null;

            if (orderStatus == 1)        //Pending
            {
                return ResponseMessage.Success;
            }

            if (orderStatus == 2)        //Processing
            {
                if (exist.OrderStatus == Constants.Status.Pending)
                {
                    orderStatusUpdate = Constants.Status.Processing;
                }
                else
                {
                    throw new ApplicationException(ResponseMessage.OrderProccessedError);
                }
            }

            if (orderStatus == 3)    //Shipped
            {
                if (exist.OrderStatus == Constants.Status.Processing)
                {
                    orderStatusUpdate = Constants.Status.Shipped;
                }
                else
                {
                    throw new ApplicationException(ResponseMessage.OrderProccessedError);
                }
            }

            if (orderStatus == 4) //Canceled
            {
                if (exist.OrderStatus == Constants.Status.Shipped)
                {
                    throw new ApplicationException(ResponseMessage.OrderCanceledError);
                }
                orderStatusUpdate = Constants.Status.Canceled;
            }

            exist.OrderStatus = orderStatusUpdate;
            _unitOfWork.OrderRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }
    }
}
