using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.InventoryDTOs;
using BeeStore_Repository.Enums.FilterBy;
using BeeStore_Repository.Enums.SortBy;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<Pagination<InventoryListDTO>> GetInventoryList(InventoryFilter? filterBy, string filterQuery,
                                                          InventorySortBy? sortCriteria,
                                                          bool descending, int pageIndex, int pageSize);
        Task<Pagination<InventoryListDTO>> GetInventoryList(int userId, InventoryFilter? filterBy, string filterQuery,
                                                          InventorySortBy? sortCriteria,
                                                          bool descending, int pageIndex, int pageSize);
        Task<InventoryListPackagesDTO> GetInventoryById(int id);

        Task<string> CreateInventory(InventoryCreateDTO request);
        Task<string> UpdateInventory(InventoryUpdateDTO request);
        Task<string> DeleteInventory(int id);
        Task<string> AddPartnerToInventory(int id, int userId);
    }
}
