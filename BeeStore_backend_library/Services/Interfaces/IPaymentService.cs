using BeeStore_Repository.DTO.PaymentDTOs;
using BeeStore_Repository.Enums;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDTO> CreateQrCode(CoinPackValue options, string custom_amount, PaymentRequestDTO request);
        Task<string> ConfirmPayment(ConfirmPaymentDTO request);
        Task<List<PaymentListDTO>> GetPaymentList(int warehouseId);
        Task<string> CreateMoneyTransferRequest(int ocopPartnerId, decimal amount);
        Task<string> ConfirmMoneyTransferRequest(int staffId, int moneyTransferRequest, string picture_link);
        Task<List<MoneyTransferListDTO>> GetMoneyTransferList(int? warehouseId);
        Task<List<MoneyTransferListDTO>> GetPartnerMoneyTransferList(int? partnerId);
        Task<List<TransactionListDTO>> GetTransactionList(int? partnerId);
    }
}
