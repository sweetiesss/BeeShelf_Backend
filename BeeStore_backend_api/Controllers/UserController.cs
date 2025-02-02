﻿using BeeStore_Repository.DTO.UserDTOs;
using BeeStore_Repository.Enums;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace BeeStore_Api.Controllers
{

    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IJWTService _jwtService;

        public UserController(IUserService userService, IJWTService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [Route("get-manager-dashboard")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetManagerDashboard(int? day, int? month, int? year)
        {
            var result = await _userService.GetManagerDashboard(day, month, year);
            return Ok(result);
        }

        [Route("get-warehouse-total-revenue/{warehouseId}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetWarehousesTotalRevenue(int warehouseId, int? year)
        {
            var result = await _userService.GetManagerTotalRevenue(warehouseId, year);
            return Ok(result);
        }


        [Route("get-employees")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetEmployees([FromQuery][DefaultValue(null)] string? search,
                                                  [FromQuery] UserSortBy? sortBy,
                                                  [FromQuery] EmployeeRole? filterByRole,
                                                  [FromQuery][DefaultValue(false)] bool descending,
                                                  [FromQuery][DefaultValue(0)] int pageIndex,
                                                  [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _userService.GetAllEmployees(search!, filterByRole, sortBy, descending, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-employee/{email}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff,Partner,Shipper")]
        public async Task<IActionResult> GetEmployee(string email)
        {
            var result = await _userService.GetEmployee(email);
            return Ok(result);
        }

        [Route("create-employee")]
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeCreateRequest user)
        {
            var result = await _userService.CreateEmployee(user);
            return Ok(result);
        }

        [Route("update-employee")]
        [HttpPut]
        [Authorize(Roles = "Admin,Manager,Staff,Shipper")]
        public async Task<IActionResult> UpdateEmployee([FromBody] EmployeeUpdateRequest user)
        {
            var result = await _userService.UpdateEmployee(user);
            return Ok(result);
        }

        [Route("delete-employee/{id}")]
        [HttpDelete]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var result = await _userService.DeleteEmployee(id);
            return Ok(result);
        }

        [Route("get-provinces")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProvinces([FromQuery][DefaultValue(0)] int pageIndex,
                                                  [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _userService.GetProvinces(pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-province/{id}")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProvince(int id)
        {
            var result = await _userService.GetProvince(id);
            return Ok(result);
        }
    }
}
