using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.WarehouseCategoryDTOs;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IWarehouseCategoryService
    {
        Task<Pagination<WarehouseCategoryListDTO>> GetWarehouseCategoryList(int pageIndex, int pageSize);
        Task<Pagination<WarehouseCategoryListDTO>> GetWarehouseCategoryList(int id, int pageIndex, int pageSize);
        Task<string> AddCategoryToWarehouse(List<WarehouseCategoryCreateDTO> request);

    }
}
