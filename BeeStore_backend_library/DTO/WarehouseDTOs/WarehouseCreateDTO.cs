using System.Text.Json.Serialization;

namespace BeeStore_Repository.DTO.WarehouseDTOs
{
    public class WarehouseCreateDTO
    {

        public string? Name { get; set; }

        public int? Capacity { get; set; }

        public string? Type { get; set; }

        public string? Location { get; set; }

        [JsonIgnore]
        public DateTime? CreateDate { get; set; }
        public List<DeliveryZoneCreateDTO>? DeliveryZones { get; set;} = new List<DeliveryZoneCreateDTO>();

    }

    public class DeliveryZoneCreateDTO
    {
        public string? Name { get; set; }
        public string? Location { get; set; }
        [JsonIgnore]
        public int? WarehouseId { get; set; }
    }
}
