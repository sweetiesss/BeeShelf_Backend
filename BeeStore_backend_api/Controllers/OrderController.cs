using BeeStore_Repository.DTO.OrderDTOs;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel;
using ZstdSharp.Unsafe;

namespace BeeStore_Api.Controllers
{

    [Authorize]
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Route("get-orders")]
        [HttpGet]
        public async Task<IActionResult> GetOrderList([FromQuery][DefaultValue(0)] int pageIndex,
                                                               [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _orderService.GetOrderList(pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-orders/{userId}")]
        [HttpGet]
        public async Task<IActionResult> GetOrderList(int userId, [FromQuery][DefaultValue(0)] int pageIndex,
                                                               [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _orderService.GetOrderList(userId, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-shipper-orders/{userId}")]
        [HttpGet]
        public async Task<IActionResult> GetShipperOrderList(int userId, [FromQuery][DefaultValue(0)] int pageIndex,
                                                               [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _orderService.GetDeliverOrderList(userId, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("create-order")]
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderCreateDTO request)
        {
            var result = await _orderService.CreateOrder(request);
            return Ok(result);
        }

        [Route("update-order/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateOrder(int id, OrderCreateDTO request)
        {
            var result = await _orderService.UpdateOrder(id, request);
            return Ok(result);
        }

        [Route("update-order-status/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateOrderStatus(int id, int orderStatus)
        {
            var result = await _orderService.UpdateOrderStatus(id, orderStatus);
            return Ok(result);
        }

        [Route("delete-order/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _orderService.DeleteOrder(id);
            return Ok(result);
        }
    }
}