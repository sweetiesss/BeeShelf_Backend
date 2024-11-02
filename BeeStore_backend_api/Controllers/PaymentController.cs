using BeeStore_Repository.DTO.PaymentDTOs;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BeeStore_Api.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [Route("create-qrcode")]
        [HttpPost]
        public async Task<IActionResult> CreatePayment(PaymentRequestDTO paymentRequestDTO)
        {
            var result = await _paymentService.CreateQrCode(paymentRequestDTO);
            return Ok(result);
        }
    }
}
