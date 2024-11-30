using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO.PaymentDTOs
{
    public class MoneyTransferListDTO
    {
        public int Id { get; set; }

        public int? OrderId { get; set; }

        public int? PaymentId { get; set; }

        public int? OcopPartnerId { get; set; }

        public int? TransferBy { get; set; }

        public string? TransferByStaffEmail {  get; set; }
        public string? TransferByStaffName { get; set; }

        public decimal? Amount { get; set; }

        public DateTime? CreateDate { get; set; }
    }
}
