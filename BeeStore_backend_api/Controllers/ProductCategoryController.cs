using BeeStore_Repository.DTO.ProductCategoryDTOs;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace BeeStore_Api.Controllers
{
    public class ProductCategoryController : BaseController
    {
        private readonly IProductCategoryService _productCategoryService;

        public ProductCategoryController(IProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductCategoryList([FromQuery][DefaultValue(0)] int pageIndex,
                                                                [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _productCategoryService.GetProductCategoryList(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductCategory(ProductCategoryCreateDTO request)
        {
            var result = await _productCategoryService.CreateProductCategory(request);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductCategory(int id, ProductCategoryCreateDTO request)
        {
            var result = await _productCategoryService.UpdateProductCategory(id, request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductCategory(int id)
        {
            var result = await _productCategoryService.DeleteProductCategory(id);
            return Ok(result);
        }
    }
}
