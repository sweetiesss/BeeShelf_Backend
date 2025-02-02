﻿using BeeStore_Repository.DTO.OrderDTOs;
using BeeStore_Repository.Enums;
using BeeStore_Repository.Enums.FilterBy;
using BeeStore_Repository.Enums.SortBy;
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

        [Route("get-order/{id}")]
        [Authorize(Roles = "Admin,Manager,Staff,Partner")]
        [HttpGet]
        public async Task<IActionResult> GetOrder(int id)
        {
            var result = await _orderService.GetOrder(id);
            return Ok(result);
        }

        [Route("get-orders")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [HttpGet]
        public async Task<IActionResult> GetOrderList([FromQuery] OrderFilterBy? orderFilterBy,
                                                      [FromQuery] bool? hasBatch,
                                                      [FromQuery] string? filterQuery,
                                                      [FromQuery] OrderStatus? filterByStatus,
                                                      [FromQuery] OrderSortBy? sortBy,
                                                      [FromQuery][DefaultValue(false)] bool descending,
                                                      [FromQuery][DefaultValue(0)] int pageIndex,
                                                      [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _orderService.GetOrderList(hasBatch, orderFilterBy, filterQuery, filterByStatus, sortBy, descending, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-orders/{userId}")]
        [Authorize(Roles = "Partner")]
        [HttpGet]
        public async Task<IActionResult> GetOrderList(int userId,
                                                      [FromQuery] bool? hasBatch,
                                                      [FromQuery] OrderFilterBy? orderFilterBy,
                                                      [FromQuery] string? filterQuery,
                                                      [FromQuery] OrderStatus? filterByStatus,
                                                      [FromQuery] OrderSortBy? sortBy,
                                                      [FromQuery][DefaultValue(false)] bool descending,
                                                      [FromQuery][DefaultValue(0)] int pageIndex,
                                                      [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _orderService.GetOrderList(hasBatch, orderFilterBy, filterQuery, userId, filterByStatus, sortBy, descending, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-shipper-orders/{userId}")]
        [Authorize(Roles = "Admin,Manager,Shipper")]
        [HttpGet]
        public async Task<IActionResult> GetShipperOrderList(int userId,
                                                            [FromQuery] bool? hasBatch,
                                                            [FromQuery] OrderFilterBy? orderFilterBy,
                                                            [FromQuery] string? filterQuery,
                                                            [FromQuery] OrderStatus? filterByStatus,
                                                            [FromQuery] OrderSortBy? sortBy,
                                                            [FromQuery][DefaultValue(false)] bool descending,
                                                            [FromQuery][DefaultValue(0)] int pageIndex,
                                                            [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _orderService.GetDeliverOrderList(hasBatch, orderFilterBy, filterQuery, userId, filterByStatus, sortBy, descending, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-store-orders")]
        [Authorize(Roles = "Admin,Manager,Staff,Shipper")]
        [HttpGet]
        public async Task<IActionResult> GetStoreSentOrder(int storeId,
                                                            [FromQuery] bool? hasBatch,
                                                            [FromQuery] OrderFilterBy? orderFilterBy,
                                                            [FromQuery] string? filterQuery,
                                                            [FromQuery] OrderStatus? filterByStatus,
                                                            [FromQuery] OrderSortBy? sortBy,
                                                            [FromQuery][DefaultValue(false)] bool descending,
                                                            [FromQuery][DefaultValue(0)] int pageIndex,
                                                            [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _orderService.GetStoreSentOrderList(hasBatch, orderFilterBy, filterQuery, storeId, filterByStatus, sortBy, descending, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("create-order")]
        [Authorize(Roles = "Admin,Partner")]
        [HttpPost]
        public async Task<IActionResult> CreateOrder(int warehouseId,bool send, OrderCreateDTO request)
        {
            await _orderService.CreateOrderHandler(send, request);
            return Ok("succ");
        }

        [Route("update-order/{id}")]
        [Authorize(Roles = "Partner")]
        [HttpPut]
        public async Task<IActionResult> UpdateOrder(int id, int warehouseId, OrderUpdateDTO request)
        {
            var result = await _orderService.UpdateOrder(id, warehouseId, request);
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
        public async Task<IActionResult> UpdateOrderStatus(int id, OrderStatus orderStatus, string? cancellationReason)
        {
            var result = await _orderService.UpdateOrderStatus(id, orderStatus, cancellationReason);
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