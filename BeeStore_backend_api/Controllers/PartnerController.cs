using BeeStore_Repository.DTO.PartnerDTOs;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace BeeStore_Api.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class PartnerController : BaseController
    {
        private readonly IPartnerService _partnerService;

        public PartnerController(IPartnerService partnerService)
        {
            _partnerService = partnerService;
        }

        [Route("get-partners")]
        [HttpGet]
        public async Task<IActionResult> GetPartnerList([FromQuery][DefaultValue(0)] int pageIndex,
                                                               [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _partnerService.GetPartnerList(pageIndex, pageSize);
            return Ok(result);
        }

        [Route("upgrade-to-partner")]
        [HttpPost]
        public async Task<IActionResult> UpgradeToPartner(PartnerUpdateRequest request)
        {
            var result = await _partnerService.UpgradeToPartner(request);
            return Ok(result);
        }

        [Route("update-partner")]
        [HttpPut]
        public async Task<IActionResult> UpdatePartner(PartnerUpdateRequest request)
        {
            var result = await _partnerService.UpdatePartner(request);
            return Ok(result);
        }

        [Route("delete-partner/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeletePartner(int id)
        {
            var result = await _partnerService.DeletePartner(id);
            return Ok(result);
        }
    }
}
