﻿using BeeStore_Repository.DTO.WarehouseDTOs;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using ZstdSharp.Unsafe;

namespace BeeStore_Api.Controllers
{
    public class WarehouseController : BaseController
    {
        private readonly IWarehouseService _warehouseService;
        private readonly IWarehouseCategoryService _warehouseCategoryService;
        private readonly IWarehouseShipperService _warehouseShipperService;
        private readonly IWarehouseStaffService _warehouseStaffService;

        public WarehouseController(IWarehouseService warehouseService,
                                    IWarehouseCategoryService warehouseCategoryService,
                                    IWarehouseShipperService warehouseShipperService,
                                    IWarehouseStaffService warehouseStaffService)
        {
            _warehouseService = warehouseService;
            _warehouseCategoryService = warehouseCategoryService;
            _warehouseShipperService = warehouseShipperService;
            _warehouseStaffService = warehouseStaffService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWarehouseList([FromQuery][DefaultValue(0)] int pageIndex,
                                                               [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _warehouseService.GetWarehouseList(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetWarehouseShipperList([FromQuery][DefaultValue(0)] int pageIndex,
                                                               [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _warehouseShipperService.GetWarehouseShipperList(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetWarehouseStaffList([FromQuery][DefaultValue(0)] int pageIndex,
                                                               [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _warehouseStaffService.GetWarehouseStaffList(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetWarehouseCategoryList([FromQuery][DefaultValue(0)] int pageIndex,
                                                               [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _warehouseCategoryService.GetWarehouseCategoryList(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWarehouse(WarehouseCreateDTO request)
        {
            var result = await _warehouseService.CreateWarehouse(request);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateWarehouse(WarehouseCreateDTO request)
        {
            var result = await _warehouseService.UpdateWarehouse(request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarehouse(int id)
        {
            var result = await _warehouseService.DeleteWarehouse(id);
            return Ok(result);
        }
    }
}
