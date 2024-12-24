using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace BeeStore_Repository.DTO.PartnerDTOs
{
    public class PartnerVerificationPaperCreateDTO
    {
        public int? OcopPartnerId { get; set; }

        [JsonIgnore]
        public DateTime? CreateDate { get; set; } = DateTime.Now;
        [JsonIgnore]
        public ulong? IsVerified { get; set; } = 0;
        [JsonIgnore]
        public DateTime? VerifyDate { get; set; }
        [JsonIgnore]
        public ulong? IsRejected { get; set; } = 0;
        [JsonIgnore]
        public DateTime? RejectDate { get; set; }
        [JsonIgnore]
        public string? RejectReason { get; set; }
        [JsonIgnore]
        public string? FrontPictureLink { get; set; }
        [JsonIgnore]
        public string? BackPictureLink { get; set; }

        //public IFormFile ocop_paper {  get; set; }
        //public IFormFile business_paper { get; set; }

    }
}
