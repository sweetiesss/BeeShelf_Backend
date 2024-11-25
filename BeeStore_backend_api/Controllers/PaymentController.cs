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
        public async Task<IActionResult> CreatePayment([FromQuery] CoinPackValue options,
                                                       [FromQuery][DefaultValue(null)] string? custom_amount,
                                                       [FromBody] PaymentRequestDTO paymentRequestDTO)
        {
            var result = await _paymentService.CreateQrCode(options, custom_amount, paymentRequestDTO);
            return Ok(result);
        }

        [AllowAnonymous]
        [Route("confirm-payment")]
        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(ConfirmPaymentDTO request)
        {
            var result = await _paymentService.ConfirmPayment(request);
            return Ok(result);
        }

        [Authorize(Roles = "Admin, Staff")]
        [Route("get-payments")]
        [HttpGet]
        public async Task<IActionResult> GetPayment()
        {
            var result = await _paymentService.GetPaymentList();
            return Ok(result);
        }

        [Authorize(Roles = "Admin, Staff")]
        [Route("create-money-transfer/{paymentId}")]
        [HttpPost]
        public async Task<IActionResult> CreateMoneyTransfer(int paymentId)
        {
            var result = await _paymentService.CreateMoneyTransfer(paymentId);
            return Ok(result);
        }
    }
}
