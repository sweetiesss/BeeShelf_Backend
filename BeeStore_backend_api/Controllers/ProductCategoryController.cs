using BeeStore_Repository.DTO.ProductCategoryDTOs;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel;

namespace BeeStore_Api.Controllers
{
    public class ProductCategoryController : BaseController
    {
        private readonly IProductCategoryService _productCategoryService;
        private readonly IMemoryCache _memoryCache;
        private const string cacheKey = "productCategoryCache";

        public ProductCategoryController(IProductCategoryService productCategoryService, IMemoryCache memoryCache)
        {
            _productCategoryService = productCategoryService;
            _memoryCache = memoryCache;
        }

        [Route("get-categories")]
        [HttpGet]
        [AllowAnonymous]

        public async Task<IActionResult> GetCategoriesList([FromQuery][DefaultValue(0)] int pageIndex,
                                                                [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _productCategoryService.GetCategoryList(pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-ocop-categories")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetOcopCategories([FromQuery][DefaultValue(0)] int pageIndex,
                                                                [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _productCategoryService.GetOCOPCategoryListDTO(pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-product-categories")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Partner")]
        public async Task<IActionResult> GetProductCategoryList([FromQuery][DefaultValue(0)] int pageIndex,
                                                                [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _productCategoryService.GetProductCategoryList(pageIndex, pageSize);
            return Ok(result);
        }

        [Route("create-product-category")]
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateProductCategory(ProductCategoryCreateDTO request)
        {
            var result = await _productCategoryService.CreateProductCategory(request);
            return Ok(result);
        }

        [Route("update-product-category/{id}")]
        [HttpPut]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateProductCategory(int id, ProductCategoryCreateDTO request)
        {
            var result = await _productCategoryService.UpdateProductCategory(id, request);
            return Ok(result);
        }

        [Route("delete-product-category/{id}")]
        [HttpDelete]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteProductCategory(int id)
        {
            var result = await _productCategoryService.DeleteProductCategory(id);
            return Ok(result);
        }
    }
}
