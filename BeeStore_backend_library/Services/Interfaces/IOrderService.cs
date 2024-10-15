using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.OrderDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Pagination<OrderListDTO>> GetOrderList(int pageIndex, int pageSize);
        Task<Pagination<OrderListDTO>> GetOrderList(int userId , int pageIndex, int pageSize);
        Task<Pagination<OrderListDTO>> GetDeliverOrderList(int userId, int pageIndex, int pageSize);

        Task<OrderCreateDTO> CreateOrder(OrderCreateDTO request);
        Task<string> UpdateOrderStatus(int id, int orderStatus);
        Task<OrderCreateDTO> UpdateOrder(int id, OrderCreateDTO request);
        Task<string> DeleteOrder(int id);
    }
}
