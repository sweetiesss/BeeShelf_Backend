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
        [Route("get-payments/{warehouseId}")]
        [HttpGet]
        public async Task<IActionResult> GetPayment(int warehouseId)
        {
            var result = await _paymentService.GetPaymentList(warehouseId);
            return Ok(result);
        }

        [Authorize(Roles = "Admin, Staff")]
        [Route("get-money-transfers/{partnerId}")]
        [HttpGet]
        public async Task<IActionResult> GetMoneyTransfer(int? partnerId)
        {
            var result = await _paymentService.GetMoneyTransferList();
            return Ok(result);
        }

        [Authorize(Roles = "Partner")]
        [Route("create-money-transfer-request/{partnerId}")]
        [HttpPost]
        public async Task<IActionResult> CreateMoneyTransfer(int partnerId, decimal amount)
        {
            var result = await _paymentService.CreateMoneyTransferRequest(partnerId, amount);
            return Ok(result);
        }

        [Authorize(Roles = "Admin, Staff")]
        [Route("confirm-money-transfer-request/{staffId}/{moneyTransferId}")]
        [HttpPost]
        public async Task<IActionResult> ConfirmMoneyTransferRequest(int staffId, int moneyTransferId)
        {
            var result = await _paymentService.ConfirmMoneyTransferRequest(staffId, moneyTransferId);
            return Ok(result);
        }
    }
}
