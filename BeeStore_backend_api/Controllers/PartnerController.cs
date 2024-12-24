using BeeStore_Repository.DTO.PartnerDTOs;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace BeeStore_Api.Controllers
{
    public class PartnerController : BaseController
    {
        private readonly IPartnerService _partnerService;
        private readonly IWalletService _walletService;
        public PartnerController(IPartnerService partnerService, IWalletService walletService)
        {
            _partnerService = partnerService;
            _walletService = walletService;
        }

        [Route("get-partners")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> GetPartnerList([FromQuery] string? search,
                                                        [FromQuery] SortBy? sortBy,
                                                        [FromQuery][DefaultValue(false)] bool descending,
                                                        [FromQuery][DefaultValue(0)] int pageIndex,
                                                        [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _partnerService.GetAllPartners(search, sortBy, descending, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-partner/{email}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff,Partner")]
        public async Task<IActionResult> GetPartner(string email)
        {
            var result = await _partnerService.GetPartner(email);
            return Ok(result);
        }

        [Route("get-provinces")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProvince()
        {
            var result = await _partnerService.GetProvince();
            return Ok(result);
        }

        [Route("get-wallet/{userId}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff,Partner")]
        public async Task<IActionResult> GetWalletByUserId(int userId)
        {
            var result = await _walletService.GetWalletByUserId(userId);
            return Ok(result);
        }

        [Route("update-partner")]
        [HttpPut]
        [Authorize(Roles = "Admin,Manager,Partner")]
        [Authorize(Roles = "Partner")]
        public async Task<IActionResult> UpdatePartner(OCOPPartnerUpdateRequest request)
        {
            var result = await _partnerService.UpdatePartner(request);
            return Ok(result);
        }

        [Route("delete-partner/{id}")]
        [HttpDelete]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeletePartner(int id)
        {
            var result = await _partnerService.DeletePartner(id);
            return Ok(result);
        }

        [Route("get-partner-revenue/{id}")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPartnerRevenue(int id, int? year)
        {
            var result = await _partnerService.GetPartnerRevenue(id, year);
            return Ok(result);
        }

        [Route("get-partner-total-products/{id}")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPartnerTotalProduct(int id, int? storeId)
        {
            var result = await _partnerService.GetPartnerTotalProduct(id, storeId);
            return Ok(result);
        }

        [Route("get-verification-paper/{partnerId}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Partner")]
        public async Task<IActionResult> GetPartnerVerificationPaper(int partnerId)
        {
            var result = await _partnerService.GetPartnerVerificationPaper(partnerId);
            return Ok(result);
        }

        [Route("get-verification-papers")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetPartnerVerificationPaper(bool? verified, 
                                                                    [FromQuery][DefaultValue(0)] int pageIndex,
                                                                    [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _partnerService.GetAllPartnerVerificationPaper(verified, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("create-verification-paper")]
        [HttpPost]
        [Authorize(Roles = "Admin, Partner")]
        public async Task<IActionResult> CreatePartnerVerificationPapers(int ocop_partner_id,
                                                                        List<IFormFile> file)
        {
            var result = await _partnerService.CreatePartnerVerificationPaper(ocop_partner_id, file);
            return Ok(result);
        }


        [Route("verify-verification-paper")]
        [HttpPut]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> VerifyVerificationPaper(int verificationPaperId)
        {
            var result = await _partnerService.VerifyPartnerVerificationPaper(verificationPaperId);
            return Ok(result);
        }

        [Route("reject-verification-paper")]
        [HttpPut]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> RejectVerificationPaper(int verificationPaperId, string reason)
        {
            var result = await _partnerService.RejectPartnerVerificationPaper(verificationPaperId, reason);
            return Ok(result);
        }
    }
}
