using System.Text.Json.Serialization;

namespace BeeStore_Repository.DTO.OrderDTOs
{
    public class OrderCreateDTO
    {

        public int? OcopPartnerId { get; set; }

        public string? ReceiverPhone { get; set; }

        public string? ReceiverAddress { get; set; }
        public decimal? Distance { get; set; }
        public int? DeliveryZoneId { get; set; }




        [JsonIgnore]
        public List<OrderDetailCreateDTO>? OrderDetails { get; set; } = new List<OrderDetailCreateDTO>();
        public List<ProductDetailDTO> Products { get; set; }

    }

    public class ProductDetailDTO
    {
        public int ProductId { get; set; }
        public int ProductAmount { get; set; }


    }


    public class OrderDetailCreateDTO
    {
        [JsonIgnore]
        public int? OrderId { get; set; }

        public int? LotId { get; set; }
        [JsonIgnore]
        public decimal? ProductPrice { get; set; }

        public int? ProductAmount { get; set; }
    }
}
