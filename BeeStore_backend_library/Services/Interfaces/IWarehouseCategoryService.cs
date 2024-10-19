using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.WarehouseCategoryDTOs;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IWarehouseCategoryService
    {
        //petition to delete warehouse category (it's fucking useless)
        //and if you are asking me "why don't you just make one apply filter list and then just call it and use it?"
        //the answer is idk how to, i will do that later, right now I just want to finish this shit
        Task<Pagination<WarehouseCategoryListDTO>> GetWarehouseCategoryList(int pageIndex, int pageSize);
        Task<Pagination<WarehouseCategoryListDTO>> GetWarehouseCategoryList(int id, int pageIndex, int pageSize);
        Task<string> AddCategoryToWarehouse(List<WarehouseCategoryCreateDTO> request);

    }
}
