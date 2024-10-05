using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.OrderDTOs;
using BeeStore_Repository.Enums;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Services
{
    public class OrderService : IOrderService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly OrderStatusEnums _ORDERSTATUS;
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

        public async Task<Pagination<OrderListDTO>> GetOrderList(string email, int pageIndex, int pageSize)
        {
            var query = await _unitOfWork.OrderRepo.GetQueryable(query => query.Include(o => o.User));
            var list = query.Where(u => u.User.Email == email).ToList();
            var result = _mapper.Map<List<OrderListDTO>>(list);
            return await ListPagination<OrderListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<Pagination<OrderListDTO>> GetDeliverOrderList(string shipperEmail, int pageIndex, int pageSize)
        {
            var query = await _unitOfWork.OrderRepo.GetQueryable(query => query.Include(o => o.DeliverByNavigation));
            var list = query.Where(u => u.DeliverByNavigation.Email == shipperEmail).ToList();
            var result = _mapper.Map<List<OrderListDTO>>(list);
            return await ListPagination<OrderListDTO>.PaginateList(result, pageIndex, pageSize);
        }



        public async Task<OrderCreateDTO> CreateOrder(OrderCreateDTO request)
        {
            //check for userId (both shipper and partner) (also check roles)
            //check for product
            
            var user = await _unitOfWork.UserRepo.SingleOrDefaultAsync(u => u.Id == request.UserId,
                                                                       query => query.Include(o => o.Role));
            if(user == null)
            {
                throw new KeyNotFoundException("Partner not found.");
            }
            if (user.Role.RoleName != "Partner")
            {
                throw new ApplicationException("User is not a partner.");
            }
            
            if(request.DeliverBy != 0)
            {
                var shipper = await _unitOfWork.UserRepo.SingleOrDefaultAsync(u => u.Id == request.DeliverBy,
                                                                       query => query.Include(o => o.Role));

                if (shipper == null)
                {
                    throw new KeyNotFoundException("Shipper not found.");
                }
                if (shipper.Role.RoleName != "Shipper")
                {
                    throw new ApplicationException("User in deliver_by is not a shipper.");
                }
            }

            var product = await _unitOfWork.ProductRepo.SingleOrDefaultAsync(u => u.Id == request.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException("No product found.");
            }
            request.CreateDate = DateTime.Now;
            request.OrderStatus = "Pending";
            var result = _mapper.Map<Order>(request);
            await _unitOfWork.OrderRepo.AddAsync(result);
            await _unitOfWork.SaveAsync();
            return request;

        }

        public async Task<string> DeleteOrder(int id)
        {
            var exist = await _unitOfWork.OrderRepo.SingleOrDefaultAsync(u =>u.Id == id);
            if(exist == null)
            {
                throw new KeyNotFoundException("Order not found.");
            }
            if (exist.OrderStatus != "Pending")
            {
                throw new ApplicationException("You can't delete processed orders.");
            }
            _unitOfWork.OrderRepo.SoftDelete(exist);
            await _unitOfWork.SaveAsync();
            return "Success";
        }

        public async Task<OrderCreateDTO> UpdateOrder(int id, OrderCreateDTO request)
        {
            var exist = await _unitOfWork.OrderRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException("Order not found.");
            }
            if (exist.OrderStatus != "Pending")
            {
                throw new ApplicationException("You can't edit processed orders.");
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
            return request;
        }

        public async Task<string> UpdateOrderStatus(int id, int orderStatus)
        {
            var exist = await _unitOfWork.OrderRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException("Order not found.");
            }
            string orderStatusUpdate = null;

            //if (exist.OrderStatus != "Pending")
            //{
            //    throw new ApplicationException("You can't edit already processed orders.");
            //}
            
            if (orderStatus == 1)        //Pending
            {
                return "Success";
            }
            if(orderStatus == 2)        //Processing
            {
                if(exist.OrderStatus == "Pending")
                {
                    orderStatusUpdate = "Proccessing";
                }
                else
                {
                    throw new ApplicationException("You can't edit already processed orders.");
                }
            }
            if(orderStatus == 3)    //Shipped
            {
                if (exist.OrderStatus == "Processing")
                {
                    orderStatusUpdate = "Shipped";
                }
                else
                {
                    throw new ApplicationException("You can't edit unprocess or finished orders.");
                }
            }
            if(orderStatus == 4) //Canceled
            {
                if(exist.OrderStatus == "Shipped")
                {
                    throw new ApplicationException("You can't canceled finished orders.");
                }
                orderStatusUpdate = "Canceled";
            } 




            exist.OrderStatus = orderStatusUpdate;
            _unitOfWork.OrderRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return "Success";
        }
    }
}
