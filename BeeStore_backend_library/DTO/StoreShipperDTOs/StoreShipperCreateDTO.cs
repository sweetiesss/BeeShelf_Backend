using System.Text.Json.Serialization;

namespace BeeStore_Repository.DTO.WarehouseShipperDTOs
{
    public class StoreShipperCreateDTO
    {
        public int EmployeeId { get; set; }
        public int WarehouseId { get; set; }
        [JsonIgnore]
        public string? Status { get; set; }

    }
}
