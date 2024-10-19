﻿using BeeStore_Repository.DTO.WarehouseCategoryDTOs;
using BeeStore_Repository.DTO.WarehouseDTOs;
using BeeStore_Repository.DTO.WarehouseShipperDTOs;
using BeeStore_Repository.DTO.WarehouseStaffDTOs;
using BeeStore_Repository.Enums.FilterBy;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel;

namespace BeeStore_Api.Controllers
{
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
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetWarehouseList([FromQuery][DefaultValue(null)] string? search,
                                                          [FromQuery] WarehouseFilter? filterBy, string? filterQuery,
                                                          [FromQuery] WarehouseSortBy? sortCriteria,
                                                          [FromQuery][DefaultValue(false)] bool descending, 
                                                          [FromQuery][DefaultValue(0)] int pageIndex,
                                                          [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _warehouseService.GetWarehouseList(search, filterBy, filterQuery, sortCriteria, descending, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-warehouse-by-user/{userId}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff,Partner,Shipper")]
        public async Task<IActionResult> GetWarehouseByUserId(int userId)
        {
            var result = await _warehouseService.GetWarehouseByUserId(userId);
            return Ok(result);
        }


        [Route("get-warehouse-shippers")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetWarehouseShipperList([FromQuery][DefaultValue(null)] string? search,
                                                                 [FromQuery] WarehouseFilter? filterBy,
                                                                 [FromQuery][DefaultValue(null)] string? filterQuery,
                                                                 [FromQuery][DefaultValue(0)] int pageIndex,
                                                                 [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _warehouseShipperService.GetWarehouseShipperList(search, filterBy, filterQuery, pageIndex, pageSize);
            return Ok(result);
        }

        //these are probably unecessary now that we have filter
        [Route("get-warehouse-shippers/{id}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetWarehouseShipperList(int id,
                                                                 [FromQuery][DefaultValue(null)] string? search,
                                                                 [FromQuery] WarehouseFilter? filterBy,
                                                                 [FromQuery][DefaultValue(null)] string? filterQuery, 
                                                                 [FromQuery][DefaultValue(0)] int pageIndex,
                                                                 [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _warehouseShipperService.GetWarehouseShipperList(id, search, filterBy, filterQuery, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("add-shippers-to-warehouse")]
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddWarehouseShipper(List<WarehouseShipperCreateDTO> request)
        {
            var result = await _warehouseShipperService.AddShipperToWarehouse(request);
            return Ok(result);
        }

        [Route("get-warehouse-staffs")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetWarehouseStaffList([FromQuery][DefaultValue(null)] string? search,
                                                               [FromQuery] WarehouseFilter? filterBy,
                                                               [FromQuery][DefaultValue(null)] string? filterQuery,
                                                               [FromQuery][DefaultValue(0)] int pageIndex,
                                                               [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _warehouseStaffService.GetWarehouseStaffList(search, filterBy, filterQuery, pageIndex, pageSize);
            return Ok(result);
        }

        //these are probably unecessary now that we have filter
        [Route("get-warehouse-staffs/{id}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetWarehouseStaffList(int id,
                                                              [FromQuery][DefaultValue(null)] string? search,
                                                              [FromQuery] WarehouseFilter? filterBy,
                                                              [FromQuery][DefaultValue(null)] string? filterQuery, 
                                                              [FromQuery][DefaultValue(0)] int pageIndex,
                                                              [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _warehouseStaffService.GetWarehouseStaffList(id, search, filterBy, filterQuery, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("add-staffs-to-warehouse")]
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
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
            var result = await _warehouseCategoryService.GetWarehouseCategoryList(pageIndex, pageSize);
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
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddWarehouseCategory(List<WarehouseCategoryCreateDTO> request)
        {
            var result = await _warehouseCategoryService.AddCategoryToWarehouse(request);
            return Ok(result);
        }

        [Route("create-warehouse")]
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateWarehouse(WarehouseCreateDTO request)
        {
            var result = await _warehouseService.CreateWarehouse(request);
            return Ok(result);
        }

        [Route("update-warehouse/{id}")]
        [HttpPut]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateWarehouse(int id, WarehouseCreateDTO request)
        {
            var result = await _warehouseService.UpdateWarehouse(id, request);
            return Ok(result);
        }

        [Route("delete-warehouse/{id}")]
        [HttpDelete]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteWarehouse(int id)
        {
            var result = await _warehouseService.DeleteWarehouse(id);
            return Ok(result);
        }
    }
}
