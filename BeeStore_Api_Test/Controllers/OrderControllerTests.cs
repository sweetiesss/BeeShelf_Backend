using BeeStore_Api.Controllers;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.OrderDTOs;
using BeeStore_Repository.Enums;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BeeStore_Api_Test.Controllers
{
    public class OrderControllerTests
    {
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly OrderController _orderController;

        public OrderControllerTests()
        {
            _mockOrderService = new Mock<IOrderService>();
            _orderController = new OrderController(_mockOrderService.Object);
        }

        [Fact]
        public async Task GetOrderList_ShouldReturnOrderList()
        {
            _mockOrderService.Setup(s => s.GetOrderList(null, null, false, 0, 10))
                .ReturnsAsync(new Pagination<OrderListDTO>());

            var response = await _orderController.GetOrderList(null, null, false, 0, 10) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.IsType<Pagination<OrderListDTO>>(response.Value);
        }

        [Fact]
        public async Task GetOrderList_ByUserId_ShouldReturnOrderList()
        {
            var userId = 1;
            _mockOrderService.Setup(s => s.GetOrderList(userId, null, null, false, 0, 10))
                .ReturnsAsync(new Pagination<OrderListDTO>());

            var response = await _orderController.GetOrderList(userId, null, null, false, 0, 10) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.IsType<Pagination<OrderListDTO>>(response.Value);
        }

        [Fact]
        public async Task CreateOrder_ShouldReturnCreatedOrder()
        {
            var newOrder = new OrderCreateDTO();
            _mockOrderService.Setup(s => s.CreateOrder(1, newOrder)).ReturnsAsync(ResponseMessage.Success);

            var response = await _orderController.CreateOrder(1, newOrder) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }

        [Fact]
        public async Task UpdateOrder_ShouldReturnUpdatedOrder()
        {
            var updatedOrder = new OrderUpdateDTO();
            _mockOrderService.Setup(s => s.UpdateOrder(1, updatedOrder)).ReturnsAsync(ResponseMessage.Success);

            var response = await _orderController.UpdateOrder(1, updatedOrder) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }

        [Fact]
        public async Task SendOrder_ShouldReturnSuccessMessage()
        {
            string successMessage = "Order sent successfully.";
            _mockOrderService.Setup(s => s.SendOrder(1)).ReturnsAsync(successMessage);

            var response = await _orderController.SendOrder(1) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(successMessage, response.Value);
        }

        [Fact]
        public async Task CancelOrder_ShouldReturnSuccessMessage()
        {
            string successMessage = "Order canceled successfully.";
            _mockOrderService.Setup(s => s.CancelOrder(1)).ReturnsAsync(successMessage);

            var response = await _orderController.CancelOrder(1) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(successMessage, response.Value);
        }

        [Fact]
        public async Task UpdateOrderStatus_ShouldReturnUpdatedStatus()
        {
            var orderStatus = OrderStatus.Shipping;
            _mockOrderService.Setup(s => s.UpdateOrderStatus(1, orderStatus)).ReturnsAsync(ResponseMessage.Success);

            var response = await _orderController.UpdateOrderStatus(1, orderStatus) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }

        [Fact]
        public async Task DeleteOrder_ShouldReturnSuccessMessage()
        {
            string successMessage = "Order deleted successfully.";
            _mockOrderService.Setup(s => s.DeleteOrder(1)).ReturnsAsync(successMessage);

            var response = await _orderController.DeleteOrder(1) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(successMessage, response.Value);
        }
    }
}
