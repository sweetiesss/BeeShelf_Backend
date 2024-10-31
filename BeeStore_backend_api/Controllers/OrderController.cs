using BeeStore_Repository.DTO.OrderDTOs;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Enums;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace BeeStore_Api.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Route("get-orders")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [HttpGet]
        public async Task<IActionResult> GetOrderList([FromQuery] OrderStatus? filterByStatus,
                                                      [FromQuery] OrderSortBy? sortBy,
                                                      [FromQuery][DefaultValue(false)] bool descending,
                                                      [FromQuery][DefaultValue(0)] int pageIndex,
                                                      [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _orderService.GetOrderList(filterByStatus, sortBy, descending, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-orders/{userId}")]
        [Authorize(Roles = "Partner")]
        [HttpGet]
        public async Task<IActionResult> GetOrderList(int userId,
                                                      [FromQuery] OrderStatus? filterByStatus,
                                                      [FromQuery] OrderSortBy? sortBy,
                                                      [FromQuery][DefaultValue(false)] bool descending,
                                                      [FromQuery][DefaultValue(0)] int pageIndex,
                                                      [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _orderService.GetOrderList(userId, filterByStatus, sortBy, descending, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-shipper-orders/{userId}")]
        [Authorize(Roles = "Admin,Manager,Shipper")]
        [HttpGet]
        public async Task<IActionResult> GetShipperOrderList(int userId, 
                                                            [FromQuery] OrderStatus? filterByStatus,
                                                            [FromQuery] OrderSortBy? sortBy,
                                                            [FromQuery][DefaultValue(false)] bool descending,
                                                            [FromQuery][DefaultValue(0)] int pageIndex,
                                                            [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _orderService.GetDeliverOrderList(userId, filterByStatus, sortBy, descending, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("create-order")]
        [Authorize(Roles = "Partner")]
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderCreateDTO request)
        {
            var result = await _orderService.CreateOrder(request);
            return Ok(result);
        }

        [Route("update-order/{id}")]
        [Authorize(Roles = "Partner")]
        [HttpPut]
        public async Task<IActionResult> UpdateOrder(int id, OrderUpdateDTO request)
        {
            var result = await _orderService.UpdateOrder(id, request);
            return Ok(result);
        }

        [Route("send-order/{id}")]
        [Authorize(Roles = "Partner")]
        [HttpPut]
        public async Task<IActionResult> SendOrder(int id)
        {
            var result = await _orderService.SendOrder(id);
            return Ok(result);
        }

        [Route("cancel-order/{id}")]
        [Authorize(Roles = "Partner")]
        [HttpPut]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var result = await _orderService.CancelOrder(id);
            return Ok(result);
        }

        [Route("update-order-status/{id}")]
        [Authorize(Roles = "Admin,Manager,Staff,Shipper")]
        [HttpPut]
        public async Task<IActionResult> UpdateOrderStatus(int id, OrderStatus orderStatus)
        {
            var result = await _orderService.UpdateOrderStatus(id, orderStatus);
            return Ok(result);
        }

        [Route("delete-order/{id}")]
        [Authorize(Roles = "Partner")]
        [HttpDelete]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _orderService.DeleteOrder(id);
            return Ok(result);
        }
    }
}