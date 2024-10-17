using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.InventoryDTOs;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<Pagination<InventoryListDTO>> GetInventoryList(int pageIndex, int pageSize);
        Task<Pagination<InventoryListDTO>> GetInventoryList(int userId, int pageIndex, int pageSize);
        Task<InventoryListPackagesDTO> GetInventoryById(int id);

        Task<string> CreateInventory(InventoryCreateDTO request);
        Task<string> UpdateInventory(InventoryUpdateDTO request);
        Task<string> DeleteInventory(int id);
        Task<string> AddPartnerToInventory(int id, int userId);
    }
}
