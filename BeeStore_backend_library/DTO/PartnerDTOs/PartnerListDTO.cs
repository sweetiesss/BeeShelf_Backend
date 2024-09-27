using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO.PartnerDTOs
{
    public class PartnerListDTO
    {
        public int Id { get; set; }

        public string? CitizenIdentificationNumber { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string? BankName { get; set; }

        public string? BankAccountNumber { get; set; }

        public string? UserEmail { get; set; }

        [JsonIgnore]
        public ulong? IsDeleted { get; set; }
    }
}
