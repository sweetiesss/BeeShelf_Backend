using BeeStore_Repository.Models;
using System.Text.Json.Serialization;

namespace BeeStore_Repository.DTO.OrderDTOs
{
    public class OrderCreateDTO
    {

        public int? OcopPartnerId { get; set; }

        [JsonIgnore]
        public string? Status { get; set; }

        [JsonIgnore]
        public string? CancellationReason { get; set; }

        [JsonIgnore]
        public DateTime? CreateDate { get; set; }

        [JsonIgnore]
        public DateTime? DeliverStartDate { get; set; }

        [JsonIgnore]
        public DateTime? DeliverFinishDate { get; set; }

        [JsonIgnore]
        public DateTime? CompleteDate { get; set; }

        public string? ReceiverPhone { get; set; }

        public string? ReceiverAddress { get; set; }

        [JsonIgnore]
        public decimal? TotalPrice { get; set; }

        [JsonIgnore]
        public int? BatchId { get; set; }

        [JsonIgnore]
        public DateTime? PickDate { get; set; }

        [JsonIgnore]
        public int? PickStaffId { get; set; }

        [JsonIgnore]
        public string? PictureLink { get; set; }

        public List<OrderDetailCreateDTO> OrderDetails { get; set; }


    }


    public class OrderDetailCreateDTO
    {
        [JsonIgnore]
        public int? OrderId { get; set; }

        public int? LotId { get; set; }
        [JsonIgnore]
        public int? ProductPrice { get; set; }

        public int? ProductAmount { get; set; }
    }
}
