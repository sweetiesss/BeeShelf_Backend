using System.Text.Json.Serialization;

namespace BeeStore_Repository.DTO.PartnerDTOs
{
    public class PartnerUpdateRequest
    {


        public string? BankName { get; set; }

        public string? BankAccountNumber { get; set; }

        public string? CitizenIdentificationNumber { get; set; }

        [JsonIgnore]
        public DateTime? CreateDate { get; set; }
        [JsonIgnore]
        public DateTime? UpdateDate { get; set; }

        public int? UserId { get; set; }

    }
}
