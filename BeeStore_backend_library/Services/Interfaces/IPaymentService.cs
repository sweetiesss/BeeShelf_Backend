using BeeStore_Repository.DTO.PaymentDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDTO> CreateQrCode(PaymentRequestDTO request);
    }
}
