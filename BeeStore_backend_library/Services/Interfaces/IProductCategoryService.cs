using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.ProductCategoryDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IProductCategoryService
    {
        Task<Pagination<ProductCategoryListDTO>> GetProductCategoryList(int pageIndex, int pageSize);
        Task<ProductCategoryCreateDTO> CreateProductCategory(ProductCategoryCreateDTO request);
        Task<ProductCategoryCreateDTO> UpdateProductCategory(int id, ProductCategoryCreateDTO request);
        Task<string> DeleteProductCategory(int id);
    }
}
