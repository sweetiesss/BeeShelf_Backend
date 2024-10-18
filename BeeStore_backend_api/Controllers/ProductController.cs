using BeeStore_Repository.DTO.ProductDTOs;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace BeeStore_Api.Controllers
{
    [Authorize(Roles = "Partner")]
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [Route("get-products")]
        [HttpGet]
        public async Task<IActionResult> GetProductList([FromQuery][DefaultValue(0)] int pageIndex,
                                                        [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _productService.GetProductList(pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-products/{userId}")]
        [HttpGet]
        public async Task<IActionResult> GetProductListByEmail(int userId, [FromQuery][DefaultValue(0)] int pageIndex,
                                                    [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _productService.GetProductListByEmail(userId, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("create-product")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductCreateDTO request)
        {
            var result = await _productService.CreateProduct(request);
            return Ok(result);
        }

        [Route("create-products")]
        [HttpPost]
        public async Task<IActionResult> CreateProductRange(List<ProductCreateDTO> request)
        {
            var result = await _productService.CreateProductRange(request);
            return Ok(result);
        }

        [Route("update-product/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateProduct(int id, ProductCreateDTO request)
        {
            var result = await _productService.UpdateProduct(id, request);
            return Ok(result);
        }

        [Route("delete-product/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProduct(id);
            return Ok(result);
        }
    }
}
