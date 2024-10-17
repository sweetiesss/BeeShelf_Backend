using System.Text.Json.Serialization;

namespace BeeStore_Repository.DTO.OrderDTOs
{
    public class OrderCreateDTO
    {
        public int? UserId { get; set; }

        [JsonIgnore]
        public DateTime? CreateDate { get; set; }

        [JsonIgnore]
        public string? OrderStatus { get; set; }

        [JsonIgnore]
        public string? CancellationReason { get; set; }

        public string? ReceiverPhone { get; set; }

        public string? ReceiverAddress { get; set; }

        public int? ProductAmount { get; set; }

        public decimal? TotalPrice { get; set; }

        public string? CodStatus { get; set; }

        public int? DeliverBy { get; set; }

        public int? PictureId { get; set; }

        public int? ProductId { get; set; }
    }
}
