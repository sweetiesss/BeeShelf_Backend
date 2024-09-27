using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO.PartnerDTOs
{
    public class PartnerUpdateRequest
    {
        public string? UserEmail { get; set; }
        public string? CitizenIdentificationNumber { get; set; }
        [JsonIgnore]
        public DateTime? CreateDate { get; set; }
        [JsonIgnore]
        public DateTime? UpdateDate { get; set; }

        public string? BankName { get; set; }

        public string? BankAccountNumber { get; set; }

    }
}
