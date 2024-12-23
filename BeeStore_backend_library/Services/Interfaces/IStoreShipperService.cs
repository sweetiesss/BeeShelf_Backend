using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.WarehouseShipperDTOs;
using BeeStore_Repository.Enums.FilterBy;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IStoreShipperService
    {
        Task<Pagination<StoreShipperListDTO>> GetStoreShipperList(string? search, bool? hasDeliveryZone, bool? hasVehicle, StoreFilter? filterBy, string? filterQuery, int pageIndex, int pageSize);
        Task<Pagination<StoreShipperListDTO>> GetStoreShipperList(int id, string? search, bool? hasDeliveryZone, bool? hasVehicle, StoreFilter? filterBy, string? filterQuery, int pageIndex, int pageSize);
        Task<string> AddShipperToStore(List<StoreShipperCreateDTO> request);
        Task<string> AssignShipperToDeliveryZone(int shipperId, int deliveryZoneId);
    }
}
