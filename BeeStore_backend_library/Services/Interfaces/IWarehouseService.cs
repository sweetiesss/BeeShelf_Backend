using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.WarehouseDTOs;
using BeeStore_Repository.Enums.FilterBy;
using BeeStore_Repository.Enums.SortBy;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IWarehouseService
    {
        Task<Pagination<WarehouseListDTO>> GetWarehouseList(string? search, WarehouseFilter? filterBy, string? filterQuery,
                                                         WarehouseSortBy? sortCriteria, bool descending, int pageIndex, int pageSize);
        Task<List<WarehouseListInventoryDTO>> GetWarehouseByUserId(int userId);
        Task<string> CreateWarehouse(WarehouseCreateDTO request);
        Task<string> UpdateWarehouse(int id, WarehouseCreateDTO request);
        Task<string> DeleteWarehouse(int id);
    }
}
