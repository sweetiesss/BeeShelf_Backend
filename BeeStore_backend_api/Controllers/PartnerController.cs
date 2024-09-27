﻿using BeeStore_Repository.DTO.PartnerDTOs;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace BeeStore_Api.Controllers
{
    public class PartnerController : BaseController
    {
        private readonly IPartnerService _partnerService;

        public PartnerController(IPartnerService partnerService)
        {
            _partnerService = partnerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPartnerList([FromQuery][DefaultValue(0)] int pageIndex,
                                                               [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _partnerService.GetPartnerList(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpgradeToPartner(UpgradeToPartnerRequest request)
        {
            var result = await _partnerService.UpgradeToPartner(request);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePartner(PartnerUpdateRequest request)
        {
            var result = await _partnerService.UpdatePartner(request);
            return Ok(result);
        }
        [HttpDelete("{email}")]
        public async Task<IActionResult> DeletePartner(string email)
        {
            var result = await _partnerService.DeletePartner(email);
            return Ok(result);
        }
    }
}
