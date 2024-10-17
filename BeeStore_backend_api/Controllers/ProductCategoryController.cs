using BeeStore_Repository.DTO.ProductCategoryDTOs;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel;

namespace BeeStore_Api.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ProductCategoryController : BaseController
    {
        private readonly IProductCategoryService _productCategoryService;
        private readonly IMemoryCache _memoryCache;
        private const string cacheKey = "productCategoryCache";

        public ProductCategoryController(IProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }

        [Route("get-product-categories")]
        [HttpGet]
        public async Task<IActionResult> GetProductCategoryList([FromQuery][DefaultValue(0)] int pageIndex,
                                                                [FromQuery][DefaultValue(10)] int pageSize)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out var result))
            {
                result = await _productCategoryService.GetProductCategoryList(pageIndex, pageSize);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _memoryCache.Set(cacheKey, result, cacheEntryOptions);
            }


            return Ok(result);
        }

        [Route("create-product-category")]
        [HttpPost]
        public async Task<IActionResult> CreateProductCategory(ProductCategoryCreateDTO request)
        {
            var result = await _productCategoryService.CreateProductCategory(request);
            return Ok(result);
        }

        [Route("update-product-category/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateProductCategory(int id, ProductCategoryCreateDTO request)
        {
            var result = await _productCategoryService.UpdateProductCategory(id, request);
            return Ok(result);
        }

        [Route("delete-product-category/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteProductCategory(int id)
        {
            var result = await _productCategoryService.DeleteProductCategory(id);
            return Ok(result);
        }
    }
}
