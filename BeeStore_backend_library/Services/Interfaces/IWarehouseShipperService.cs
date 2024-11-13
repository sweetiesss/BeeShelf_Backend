using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.WarehouseShipperDTOs;
using BeeStore_Repository.Enums.FilterBy;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IWarehouseShipperService
    {
        Task<Pagination<WarehouseShipperListDTO>> GetWarehouseShipperList(string? search, WarehouseFilter? filterBy, string? filterQuery, int pageIndex, int pageSize);
        Task<Pagination<WarehouseShipperListDTO>> GetWarehouseShipperList(int id, string? search, WarehouseFilter? filterBy, string? filterQuery, int pageIndex, int pageSize);
        Task<string> AddShipperToWarehouse(List<WarehouseShipperCreateDTO> request);
    }
}
