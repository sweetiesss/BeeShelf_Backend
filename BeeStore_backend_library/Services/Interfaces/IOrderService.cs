using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.OrderDTOs;
using BeeStore_Repository.Enums;
using BeeStore_Repository.Enums.FilterBy;
using BeeStore_Repository.Enums.SortBy;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderListDTO> GetOrder(int id);
        Task<Pagination<OrderListDTO>> GetOrderList(bool? hasBatch, OrderFilterBy? orderFilterBy, string? filterQuery, OrderStatus? orderStatus, OrderSortBy? sortCriteria,
                                                          bool descending, int pageIndex, int pageSize);

        Task<Pagination<OrderListDTO>> GetWarehouseSentOrderList(bool? hasBatch, OrderFilterBy? orderFilterBy, string? filterQuery, int warehouseId, OrderStatus? orderStatus, OrderSortBy? sortCriteria,
                                                          bool descending, int pageIndex, int pageSize);
        Task<Pagination<OrderListDTO>> GetOrderList(bool? hasBatch, OrderFilterBy? orderFilterBy, string? filterQuery, int partner, OrderStatus? orderStatus, OrderSortBy? sortCriteria,
                                                          bool descending, int pageIndex, int pageSize);
        Task<Pagination<OrderListDTO>> GetDeliverOrderList(bool? hasBatch, OrderFilterBy? orderFilterBy, string? filterQuery, int employeeId, OrderStatus? orderStatus, OrderSortBy? sortCriteria,
                                                          bool descending, int pageIndex, int pageSize);

        Task<string> CreateOrder(int warehouseId,bool send, OrderCreateDTO request);
        Task<string> SendOrder(int id);
        Task<string> CancelOrder(int id);
        Task<string> UpdateOrderStatus(int id, OrderStatus orderStatus, string? cancellationReason);
        Task<string> UpdateOrder(int id, int warehouseId, OrderUpdateDTO request);
        Task<string> DeleteOrder(int id);
    }
}
