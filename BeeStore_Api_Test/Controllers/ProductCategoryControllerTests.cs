using BeeStore_Api.Controllers;
using BeeStore_Repository.DTO.ProductCategoryDTOs;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace BeeStore_Api_Test.Controllers
{
    public class ProductCategoryControllerTests
    {
        private readonly Mock<IProductCategoryService> _mockProductCategoryService;
        private readonly Mock<IMemoryCache> _mockMemoryCache;
        private readonly ProductCategoryController _productCategoryController;

        public ProductCategoryControllerTests()
        {
            _mockProductCategoryService = new Mock<IProductCategoryService>();
            _mockMemoryCache = new Mock<IMemoryCache>();
            _productCategoryController = new ProductCategoryController(_mockProductCategoryService.Object, _mockMemoryCache.Object);
        }

        [Fact]
        public async Task GetProductCategoryList_ShouldReturnCategoryList()
        {
            var cachedCategories = new object();
            _mockMemoryCache.Setup(m => m.TryGetValue(It.IsAny<object>(), out cachedCategories)).Returns(true);
            var response = await _productCategoryController.GetProductCategoryList(0, 10) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(cachedCategories, response.Value);
        }

        [Fact]
        public async Task CreateProductCategory_ShouldReturnCreatedCategory()
        {
            var newCategory = new ProductCategoryCreateDTO();
            _mockProductCategoryService.Setup(s => s.CreateProductCategory(newCategory)).ReturnsAsync(ResponseMessage.Success);

            var response = await _productCategoryController.CreateProductCategory(newCategory) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }

        [Fact]
        public async Task UpdateProductCategory_ShouldReturnUpdatedCategory()
        {
            var updatedCategory = new ProductCategoryCreateDTO();
            _mockProductCategoryService.Setup(s => s.UpdateProductCategory(1, updatedCategory)).ReturnsAsync(ResponseMessage.Success);

            var response = await _productCategoryController.UpdateProductCategory(1, updatedCategory) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }

        [Fact]
        public async Task DeleteProductCategory_ShouldReturnSuccessMessage()
        {
            _mockProductCategoryService.Setup(s => s.DeleteProductCategory(1)).ReturnsAsync(ResponseMessage.Success);

            var response = await _productCategoryController.DeleteProductCategory(1) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }
    }
}
