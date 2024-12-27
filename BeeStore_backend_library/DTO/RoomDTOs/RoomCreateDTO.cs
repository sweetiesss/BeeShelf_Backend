using System.Text.Json.Serialization;

namespace BeeStore_Repository.DTO.InventoryDTOs
{
    public class RoomCreateDTO
    {

        public ulong? IsCold { get; set; }
        [JsonIgnore]
        public string? RoomCode { get; set; }
        public decimal? MaxWeight { get; set; }
        public decimal? Price { get; set; }
        public int? InventoryAmount { get; set; }

        [JsonIgnore]
        public DateTime? BoughtDate { get; set; }

        [JsonIgnore]
        public DateTime? ExpirationDate { get; set; }

        public int WarehouseId { get; set; }

    }
}
