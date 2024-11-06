using BeeStore_Repository.DTO.PaymentDTOs;
using BeeStore_Repository.Enums;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;


namespace BeeStore_Api.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        [AllowAnonymous]
        [Route("create-qrcode")]
        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromQuery]CoinPackValue options, 
                                                       [FromQuery][DefaultValue(null)]string? custom_amount, 
                                                       [FromBody]PaymentRequestDTO paymentRequestDTO)
        {
            var result = await _paymentService.CreateQrCode(options, custom_amount, paymentRequestDTO);
            return Ok(result);
        }
    }
}
