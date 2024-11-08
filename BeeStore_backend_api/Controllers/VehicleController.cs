﻿using BeeStore_Repository.DTO.VehicleDTOs;
using BeeStore_Repository.Enums;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BeeStore_Api.Controllers
{
    public class VehicleController : BaseController
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [Route("get-vehicles")]
        [HttpGet]
        [Authorize(Roles ="Admin,Manager,Staff")]
        public async Task<IActionResult> GetVehicles([FromQuery]VehicleStatus? status,
                                                     [FromQuery]VehicleType? type,
                                                     [FromQuery]VehicleSortBy? sortBy,
                                                     [FromQuery][DefaultValue(false)]bool descending,
                                                     [FromQuery][DefaultValue(0)] int pageIndex,
                                                     [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _vehicleService.GetVehicles(status, type, sortBy, descending, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-vehicle/{id}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> GetVehicle(int id)
        {
            var result = await _vehicleService.GetVehicle(id); 
            return Ok(result);
        }

        [Route("create-vehicle")]
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateVehicle([FromQuery]VehicleType? type, VehicleCreateDTO request)
        {
            var result = await _vehicleService.CreateVehicle(type, request);
            return Ok(result);
        }

        [Route("update-vehicle/{id}")]
        [HttpPut]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateVehicle(int id, [FromQuery] VehicleType? type, VehicleCreateDTO request)
        {
            var result = await _vehicleService.UpdateVehicle(id, type, request);
            return Ok(result);
        }

        [Route("delete-vehicle/{id}")]
        [HttpDelete]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var result = await _vehicleService.DeleteVehicle(id);
            return Ok(result);
        }

        [Route("update-vehicle-status/{id}")]
        [HttpPut]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> UpdateVehicleStatus(int id, [FromQuery]VehicleStatus? status)
        {
            var result = await _vehicleService.UpdateVehicleStatus(id, status);
            return Ok(result);
        }

        [Route("assign-driver/{id}/{driverId}")]
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Staff")]

        public async Task<IActionResult> AssignDrier(int id, int driverId)
        {
            var result = await _vehicleService.AssignVehicle(id, driverId);
            return Ok(result);
        }
    }
}
