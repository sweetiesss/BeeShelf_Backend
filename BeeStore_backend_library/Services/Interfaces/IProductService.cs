using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.InventoryDTOs;
using BeeStore_Repository.DTO.ProductDTOs;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IProductService
    {
        Task<Pagination<ProductListDTO>> GetProductList(int pageIndex, int pageSize);
        Task<Pagination<ProductListDTO>> GetProductListByEmail(string email, int pageIndex, int pageSize);
        Task<ProductCreateDTO> CreateProduct(ProductCreateDTO request);
        Task<List<ProductCreateDTO>> CreateProductRange(List<ProductCreateDTO> request);
        Task<ProductCreateDTO> UpdateProduct(int id, ProductCreateDTO request);
        Task<string> DeleteProduct(int id);
    }
}
