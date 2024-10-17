using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.OrderDTOs;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Pagination<OrderListDTO>> GetOrderList(int pageIndex, int pageSize);
        Task<Pagination<OrderListDTO>> GetOrderList(int userId, int pageIndex, int pageSize);
        Task<Pagination<OrderListDTO>> GetDeliverOrderList(int userId, int pageIndex, int pageSize);

        Task<string> CreateOrder(OrderCreateDTO request);
        Task<string> UpdateOrderStatus(int id, int orderStatus);
        Task<string> UpdateOrder(int id, OrderCreateDTO request);
        Task<string> DeleteOrder(int id);
    }
}
