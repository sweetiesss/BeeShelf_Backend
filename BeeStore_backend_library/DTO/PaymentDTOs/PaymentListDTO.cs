using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO.PaymentDTOs
{
    public class PaymentListDTO
    {
        public int Id { get; set; }

        public int? WalletId { get; set; }

        public int? OrderId { get; set; }

        public int? TotalAmount { get; set; }
        public ulong? IsDeleted { get; set; }


    }
}
