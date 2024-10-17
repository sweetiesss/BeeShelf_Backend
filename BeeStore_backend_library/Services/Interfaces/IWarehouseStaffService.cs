using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.WarehouseStaffDTOs;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IWarehouseStaffService
    {
        Task<Pagination<WarehouseStaffListDTO>> GetWarehouseStaffList(int pageIndex, int pageSize);
        Task<Pagination<WarehouseStaffListDTO>> GetWarehouseStaffList(int id, int pageIndex, int pageSize);
        Task<string> AddStaffToWarehouse(List<WarehouseStaffCreateDTO> request);
    }
}
