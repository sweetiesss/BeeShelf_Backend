using System.Text.Json.Serialization;

namespace BeeStore_Repository.DTO.WarehouseDTOs
{
    public class WarehouseCreateDTO
    {

        public string? Name { get; set; }

        public int? Capacity { get; set; }

        public string? Type { get; set; }

        public string? Location { get; set; }
        public int? ProvinceId { get; set; }

        [JsonIgnore]
        public DateTime? CreateDate { get; set; }
        

    }

    public class DeliveryZoneCreateDTO
    {
        public string? Name { get; set; }

        public int? ProvinceId { get; set; }
    }
}
