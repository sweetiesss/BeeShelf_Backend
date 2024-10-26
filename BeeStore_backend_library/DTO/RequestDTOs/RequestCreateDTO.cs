using System.Text.Json.Serialization;

namespace BeeStore_Repository.DTO.RequestDTOs
{
    public class RequestCreateDTO
    {

        public int? OcopPartnerId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public int? LotId { get; set; }

        public string? RequestType { get; set; }

        public int? SendToInventoryId { get; set; }

        [JsonIgnore]
        public DateTime? CreateDate { get; set; }

        [JsonIgnore]
        public string? Status { get; set; }

        [JsonIgnore]
        public DateTime? ApporveDate { get; set; }

        [JsonIgnore]
        public DateTime? DeliverDate { get; set; }

        [JsonIgnore]
        public string? CancellationReason { get; set; }

    }
}
