using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.ProductDTOs;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Enums;
using BeeStore_Repository.Enums.FilterBy;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IProductService
    {
        Task<Pagination<ProductListDTO>> GetProductList(string search, ProductFilter? filterBy, string? filterQuery,
                                                        ProductSortBy? sortBy, bool descending, int pageIndex, int pageSize);
        Task<Pagination<ProductListDTO>> GetProductListById(int userId, string search, ProductFilter? filterBy, string? filterQuery,
                                                            ProductSortBy? sortBy, bool descending, int pageIndex, int pageSize);
        Task<string> CreateProduct(ProductCreateDTO request);
        Task<string> CreateProductRange(List<ProductCreateDTO> request);
        Task<string> UpdateProduct(int id, ProductCreateDTO request);
        Task<string> DeleteProduct(int id);
    }
}
