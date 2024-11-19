using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.ProductCategoryDTOs;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IProductCategoryService
    {
        Task<Pagination<OcopCategoryListDTO>> GetOCOPCategoryListDTO(int pageIndex, int pageSize);
        Task<Pagination<CategoryListDTO>> GetCategoryList(int pageIndex, int pageSize); 
        Task<Pagination<ProductCategoryListDTO>> GetProductCategoryList(int pageIndex, int pageSize);
        Task<string> CreateProductCategory(ProductCategoryCreateDTO request);
        Task<string> UpdateProductCategory(int id, ProductCategoryCreateDTO request);
        Task<string> DeleteProductCategory(int id);
    }
}
