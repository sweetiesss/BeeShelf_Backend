using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.WarehouseDTOs;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IWarehouseService
    {
        Task<Pagination<WarehouseListDTO>> GetWarehouseList(int pageIndex, int pageSize);
        Task<List<WarehouseListInventoryDTO>> GetWarehouseByUserId(int userId);
        Task<string> CreateWarehouse(WarehouseCreateDTO request);
        Task<string> UpdateWarehouse(int id, WarehouseCreateDTO request);
        Task<string> DeleteWarehouse(int id);
    }
}
