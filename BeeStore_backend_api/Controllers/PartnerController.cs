using BeeStore_Repository.DTO.PartnerDTOs;
using BeeStore_Repository.Enums.SortBy;
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
        public async Task<IActionResult> GetPartnerList([FromQuery] string? search,
                                                        [FromQuery]SortBy? sortBy,
                                                        [FromQuery][DefaultValue(false)] bool descending,
                                                        [FromQuery][DefaultValue(0)] int pageIndex,
                                                        [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _partnerService.GetAllPartners(search, sortBy, descending, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-partner/{email}")]
        [HttpGet]
        public async Task<IActionResult> GetPartner(string email)
        {
            var result = await _partnerService.GetPartner(email);
            return Ok(result);
        }


        [Route("update-partner")]
        [HttpPut]
        [Authorize(Roles = "Partner")]
        public async Task<IActionResult> UpdatePartner(OCOPPartnerUpdateRequest request)
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
