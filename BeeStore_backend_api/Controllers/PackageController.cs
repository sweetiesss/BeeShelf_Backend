﻿using BeeStore_Repository.DTO.PackageDTOs;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace BeeStore_Api.Controllers
{
    public class PackageController : BaseController
    {
        private readonly IPackageService _packageService;

        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [Route("get-packages")]
        [HttpGet]
        public async Task<IActionResult> GetPackageList([FromQuery][DefaultValue(0)] int pageIndex,
                                                               [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _packageService.GetPackageList(pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-package/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetPackageById(int id)
        {
            var result = await _packageService.GetPackageById(id);
            return Ok(result);
        }

        [Route("create-package")]
        [HttpPost]
        public async Task<IActionResult> CreatePackage(PackageCreateDTO request)
        {
            var result = await _packageService.CreatePackage(request);
            return Ok(result);
        }

        [Route("update-package/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdatePackage(int id, PackageCreateDTO request)
        {
            var result = await _packageService.UpdatePackage(id, request);
            return Ok(result);
        }

        [Route("delete-package/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeletePackage(int id)
        {
            var result = await _packageService.DeletePackage(id);
            return Ok(result);
        }
    }
}
