using BeeStore_Api.Controllers;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.ProductDTOs;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Api_Test.Controllers
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly ProductController _productController;

        public ProductControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _productController = new ProductController(_mockProductService.Object);
        }

        [Fact]
        public async Task GetProductList_ShouldReturnProductList()
        {
            var products = new Pagination<ProductListDTO> ();
            _mockProductService
                .Setup(s => s.GetProductList(null, null, null, null, false, 0, 10))
                .ReturnsAsync(products);

            var response = await _productController.GetProductList(null, null, null, null, false, 0, 10) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(products, response.Value);
        }

        [Fact]
        public async Task GetProductListById_ShouldReturnProductListByUserId()
        {
            var userId = 1;
            var products = new Pagination<ProductListDTO> ();
            _mockProductService
                .Setup(s => s.GetProductListById(userId, null, null, null, null, false, 0, 10))
                .ReturnsAsync(products);

            var response = await _productController.GetProductListById(userId, null, null, null, null, false, 0, 10) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(products, response.Value);
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnCreatedProduct()
        {
            var newProduct = new ProductCreateDTO();
            _mockProductService.Setup(s => s.CreateProduct(newProduct)).ReturnsAsync(ResponseMessage.Success);

            var response = await _productController.CreateProduct(newProduct) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }

        [Fact]
        public async Task CreateProductRange_ShouldReturnCreatedProducts()
        {
            var newProducts = new List<ProductCreateDTO> { new ProductCreateDTO(), new ProductCreateDTO() };
            _mockProductService.Setup(s => s.CreateProductRange(newProducts)).ReturnsAsync(ResponseMessage.Success);

            var response = await _productController.CreateProductRange(newProducts) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnUpdatedProduct()
        {
            var productId = 1;
            var updatedProduct = new ProductCreateDTO();
            _mockProductService.Setup(s => s.UpdateProduct(productId, updatedProduct)).ReturnsAsync(ResponseMessage.Success);

            var response = await _productController.UpdateProduct(productId, updatedProduct) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnSuccessMessage()
        {
            var productId = 1;
            _mockProductService.Setup(s => s.DeleteProduct(productId)).ReturnsAsync(ResponseMessage.Success);

            var response = await _productController.DeleteProduct(productId) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }
    }
}
