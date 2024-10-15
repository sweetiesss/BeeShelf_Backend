﻿using BeeStore_Repository.DTO.WarehouseCategoryDTOs;
using BeeStore_Repository.DTO.WarehouseDTOs;
using BeeStore_Repository.DTO.WarehouseShipperDTOs;
using BeeStore_Repository.DTO.WarehouseStaffDTOs;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel;
using ZstdSharp.Unsafe;

namespace BeeStore_Api.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class WarehouseController : BaseController
    {
        private readonly IWarehouseService _warehouseService;
        private readonly IWarehouseCategoryService _warehouseCategoryService;
        private readonly IWarehouseShipperService _warehouseShipperService;
        private readonly IWarehouseStaffService _warehouseStaffService;
        private readonly IMemoryCache _memoryCache;
        private const string warehouseCacheKey = "allWarehouseCache";
        private const string categoryCacheKey = "allWarehouseCategoryCache";


        public WarehouseController(IWarehouseService warehouseService,
                                    IWarehouseCategoryService warehouseCategoryService,
                                    IWarehouseShipperService warehouseShipperService,
                                    IWarehouseStaffService warehouseStaffService,
                                    IMemoryCache memoryCache)
        {
            _warehouseService = warehouseService;
            _warehouseCategoryService = warehouseCategoryService;
            _warehouseShipperService = warehouseShipperService;
            _warehouseStaffService = warehouseStaffService;
            _memoryCache = memoryCache;
        }

        [Route("get-warehouses")]
        [HttpGet]
        public async Task<IActionResult> GetWarehouseList([FromQuery][DefaultValue(0)] int pageIndex,
                                                               [FromQuery][DefaultValue(10)] int pageSize)
        {
            if (!_memoryCache.TryGetValue(warehouseCacheKey, out var result))
            {
                result = await _warehouseService.GetWarehouseList(pageIndex, pageSize);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(15))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _memoryCache.Set(warehouseCacheKey, result, cacheEntryOptions);
            }
            return Ok(result);
        }

        [Route("get-warehouse-by-user/{userId}")]
        [HttpGet]
        public async Task<IActionResult> GetWarehouseByUserId(int userId)
        {
            var result = await _warehouseService.GetWarehouseByUserId(userId);
            return Ok(result);
        }


        [Route("get-warehouse-shippers")]
        [HttpGet]
        public async Task<IActionResult> GetWarehouseShipperList([FromQuery][DefaultValue(0)] int pageIndex,
                                                               [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _warehouseShipperService.GetWarehouseShipperList(pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-warehouse-shippers/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetWarehouseShipperList(int id, [FromQuery][DefaultValue(0)] int pageIndex,
                                                               [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _warehouseShipperService.GetWarehouseShipperList(id, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("add-shippers-to-warehouse")]
        [HttpPost]
        public async Task<IActionResult> AddWarehouseShipper(List<WarehouseShipperCreateDTO> request)
        {
            var result = await _warehouseShipperService.AddShipperToWarehouse(request);
            return Ok(result);
        }

        [Route("get-warehouse-staffs")]
        [HttpGet]
        public async Task<IActionResult> GetWarehouseStaffList([FromQuery][DefaultValue(0)] int pageIndex,
                                                               [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _warehouseStaffService.GetWarehouseStaffList(pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-warehouse-staffs/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetWarehouseStaffList(int id, [FromQuery][DefaultValue(0)] int pageIndex,
                                                              [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _warehouseStaffService.GetWarehouseStaffList(id, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("add-staffs-to-warehouse")]
        [HttpPost]
        public async Task<IActionResult> AddWarehouseStaff(List<WarehouseStaffCreateDTO> request)
        {
            var result = await _warehouseStaffService.AddStaffToWarehouse(request);
            return Ok(result);
        }

        [Route("get-warehouse-categories")]
        [HttpGet]
        public async Task<IActionResult> GetWarehouseCategoryList([FromQuery][DefaultValue(0)] int pageIndex,
                                                               [FromQuery][DefaultValue(10)] int pageSize)
        {
            if (!_memoryCache.TryGetValue(categoryCacheKey, out var result))
            {
                result = await _warehouseCategoryService.GetWarehouseCategoryList(pageIndex, pageSize);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _memoryCache.Set(categoryCacheKey, result, cacheEntryOptions);
            }
            
            return Ok(result);
        }

        [Route("get-warehouse-categories/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetWarehouseCategoryList(int id, [FromQuery][DefaultValue(0)] int pageIndex,
                                                               [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _warehouseCategoryService.GetWarehouseCategoryList(id, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("add-categories-to-warehouse")]
        [HttpPost]
        public async Task<IActionResult> AddWarehouseCategory(List<WarehouseCategoryCreateDTO> request)
        {
            var result = await _warehouseCategoryService.AddCategoryToWarehouse(request);
            return Ok(result);
        }

        [Route("create-warehouse")]
        [HttpPost]
        public async Task<IActionResult> CreateWarehouse(WarehouseCreateDTO request)
        {
            var result = await _warehouseService.CreateWarehouse(request);
            return Ok(result);
        }

        [Route("update-warehouse")]
        [HttpPut]
        public async Task<IActionResult> UpdateWarehouse(WarehouseCreateDTO request)
        {
            var result = await _warehouseService.UpdateWarehouse(request);
            return Ok(result);
        }

        [Route("delete-warehouse/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteWarehouse(int id)
        {
            var result = await _warehouseService.DeleteWarehouse(id);
            return Ok(result);
        }
    }
}
