using BeeStore_Repository.DTO.OrderDTOs;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel;
using ZstdSharp.Unsafe;

namespace BeeStore_Api.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderList([FromQuery][DefaultValue(0)] int pageIndex,
                                                               [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _orderService.GetOrderList(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetOrderList(string email, [FromQuery][DefaultValue(0)] int pageIndex,
                                                               [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _orderService.GetOrderList(email, pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetShipperOrderList(string email, [FromQuery][DefaultValue(0)] int pageIndex,
                                                               [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _orderService.GetDeliverOrderList(email, pageIndex, pageSize);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderCreateDTO request)
        {
            var result = await _orderService.CreateOrder(request);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, OrderCreateDTO request)
        {
            var result = await _orderService.UpdateOrder(id, request);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderStatus(int id, int orderStatus)
        {
            var result = await _orderService.UpdateOrderStatus(id, orderStatus);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _orderService.DeleteOrder(id);
            return Ok(result);
        }
    }
}