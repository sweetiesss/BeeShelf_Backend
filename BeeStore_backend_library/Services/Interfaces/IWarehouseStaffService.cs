using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.WarehouseStaffDTOs;
using BeeStore_Repository.Enums.FilterBy;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IWarehouseStaffService
    {
        Task<Pagination<WarehouseStaffListDTO>> GetWarehouseStaffList(string? search, WarehouseFilter? filterBy, string? filterQuery, int pageIndex, int pageSize);
        Task<Pagination<WarehouseStaffListDTO>> GetWarehouseStaffList(int id, string? search, WarehouseFilter? filterBy, string? filterQuery, int pageIndex, int pageSize);
        Task<string> AddStaffToWarehouse(List<WarehouseStaffCreateDTO> request);
    }
}
