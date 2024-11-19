using BeeStore_Repository.DTO.PaymentDTOs;
using BeeStore_Repository.Enums;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDTO> CreateQrCode(CoinPackValue options, string custom_amount, PaymentRequestDTO request);
        Task<string> ConfirmPayment(ConfirmPaymentDTO request);
    }
}
