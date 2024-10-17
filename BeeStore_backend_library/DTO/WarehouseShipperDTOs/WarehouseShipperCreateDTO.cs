using System.Text.Json.Serialization;

namespace BeeStore_Repository.DTO.WarehouseShipperDTOs
{
    public class WarehouseShipperCreateDTO
    {
        public int UserId { get; set; }
        public int WarehouseId { get; set; }
        [JsonIgnore]
        public string? Status { get; set; }

    }
}
