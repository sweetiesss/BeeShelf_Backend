﻿using BeeStore_Repository.DTO.InventoryDTOs;
using BeeStore_Repository.Enums.FilterBy;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel;

namespace BeeStore_Api.Controllers
{
    public class InventoryController : BaseController
    {
        private readonly IInventoryService _inventoryService;
        private readonly IMemoryCache _memoryCache;
        private const string cacheKey = "inventoryCache";
        public InventoryController(IInventoryService inventoryService, IMemoryCache memoryCache)
        {
            _inventoryService = inventoryService;
            _memoryCache = memoryCache;
        }

        [Route("buy-inventory/{id}/{userId}")]
        [HttpPost]
        [Authorize(Roles = "Partner")]
        public async Task<IActionResult> BuyInventory(int id, int userId, int month)
        {
            var result = await _inventoryService.BuyInventory(id, userId, month);
            return Ok(result);
        }

        [Route("extend-inventory/{id}/{userId}")]
        [HttpPost]
        [Authorize(Roles = "Partner")]
        public async Task<IActionResult> ExtendInventory(int id, int userId, int month)
        {
            var result = await _inventoryService.ExtendInventory(id, userId, month);
            return Ok(result);
        }

        [Route("get-inventories")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff,Partner")]
        public async Task<IActionResult> GetInventoryList([FromQuery] InventoryFilter? filterBy,
                                                          [FromQuery][DefaultValue(null)] string? filterQuery,
                                                          [FromQuery] InventorySortBy? sortCriteria,
                                                          [FromQuery][DefaultValue(false)] bool descending,
                                                          [FromQuery][DefaultValue(0)] int pageIndex,
                                                          [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _inventoryService.GetInventoryList(filterBy, filterQuery, sortCriteria, descending, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-inventories/{userId}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff,Partner")]
        public async Task<IActionResult> GetInventoryList(int userId,
                                                          [FromQuery] InventoryFilter? filterBy,
                                                          [FromQuery][DefaultValue(null)] string? filterQuery,
                                                          [FromQuery] InventorySortBy? sortCriteria,
                                                          [FromQuery][DefaultValue(false)] bool descending,
                                                          [FromQuery][DefaultValue(0)] int pageIndex,
                                                          [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _inventoryService.GetInventoryList(userId, filterBy, filterQuery, sortCriteria, descending, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-inventory/{id}")]
        [Authorize(Roles = "Admin,Manager,Staff,Partner")]
        [HttpGet]
        public async Task<IActionResult> GetInventoryById(int id)
        {
            var result = await _inventoryService.GetInventoryById(id);
            return Ok(result);
        }

        [Route("add-partner-to-inventory/{id}/{userId}")]
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<IActionResult> AddPartnerToInventory(int id, int userId)
        {
            var result = await _inventoryService.AddPartnerToInventory(id, userId);
            return Ok(result);
        }

        [Route("create-inventory")]
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<IActionResult> CreateInventory(InventoryCreateDTO request)
        {
            var result = await _inventoryService.CreateInventory(request);
            return Ok(result);
        }

        [Route("update-inventory")]
        [Authorize(Roles = "Admin,Manager")]
        [HttpPut]
        public async Task<IActionResult> UpdateInventory(InventoryUpdateDTO request)
        {
            var result = await _inventoryService.UpdateInventory(request);
            return Ok(result);
        }

        [Route("delete-inventory/{id}")]
        [Authorize(Roles = "Admin,Manager")]
        [HttpDelete]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            var result = await _inventoryService.DeleteInventory(id);
            return Ok(result);
        }
    }

}
