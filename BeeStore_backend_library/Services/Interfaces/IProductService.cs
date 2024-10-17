using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.ProductDTOs;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IProductService
    {
        Task<Pagination<ProductListDTO>> GetProductList(int pageIndex, int pageSize);
        Task<Pagination<ProductListDTO>> GetProductListByEmail(int userId, int pageIndex, int pageSize);
        Task<string> CreateProduct(ProductCreateDTO request);
        Task<string> CreateProductRange(List<ProductCreateDTO> request);
        Task<string> UpdateProduct(int id, ProductCreateDTO request);
        Task<string> DeleteProduct(int id);
    }
}
