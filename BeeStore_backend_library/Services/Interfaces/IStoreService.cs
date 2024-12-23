using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.WarehouseDTOs;
using BeeStore_Repository.Enums.FilterBy;
using BeeStore_Repository.Enums.SortBy;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IStoreService
    {
        Task<Pagination<StoreListDTO>> GetStoreList(string? search, StoreFilter? filterBy, string? filterQuery,
                                                         StoreSortBy? sortCriteria, bool descending, int pageIndex, int pageSize);
        Task<List<StoreListInventoryDTO>> GetStoreByUserId(int userId);
        Task<StoreDeliveryZoneDTO> GetStoreById(int id);
        Task<string> CreateStore(StoreCreateDTO request);
        Task<string> UpdateStore(int id, StoreCreateDTO request);
        Task<string> DeleteStore(int id);
    }
}
