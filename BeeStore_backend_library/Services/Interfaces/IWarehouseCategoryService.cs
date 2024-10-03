using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.WarehouseCategoryDTOs;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IWarehouseCategoryService
    {
        Task<Pagination<WarehouseCategoryListDTO>> GetWarehouseCategoryList(int pageIndex, int pageSize);

    }
}
