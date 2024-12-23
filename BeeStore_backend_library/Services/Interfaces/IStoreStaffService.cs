using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.WarehouseStaffDTOs;
using BeeStore_Repository.Enums.FilterBy;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IStoreStaffService
    {
        Task<Pagination<StoreStaffListDTO>> GetStoreStaffList(string? search, StoreFilter? filterBy, string? filterQuery, int pageIndex, int pageSize);
        Task<Pagination<StoreStaffListDTO>> GetStoreStaffList(int id, string? search, StoreFilter? filterBy, string? filterQuery, int pageIndex, int pageSize);
        Task<string> AddStaffToStore(List<StoreStaffCreateDTO> request);
    }
}
