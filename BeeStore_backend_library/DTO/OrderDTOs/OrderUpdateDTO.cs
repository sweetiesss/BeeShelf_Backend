using System.Text.Json.Serialization;

namespace BeeStore_Repository.DTO.OrderDTOs
{
    public class OrderUpdateDTO
    {
        public int? OcopPartnerId { get; set; }

        public string? ReceiverPhone { get; set; }

        public string? ReceiverAddress { get; set; }
        public decimal? Distance { get; set; }
        public int? DeliveryZoneId { get; set; }



        [JsonIgnore]
        public List<OrderDetailUpdateDTO>? OrderDetails { get; set; } = new List<OrderDetailUpdateDTO>();
        public List<ProductDetailDTO> Products { get; set; }

    }

    public class OrderDetailUpdateDTO
    {
        public int? LotId { get; set; }
        [JsonIgnore]
        public decimal? ProductPrice { get; set; }

        public int? ProductAmount { get; set; }
    }
}
