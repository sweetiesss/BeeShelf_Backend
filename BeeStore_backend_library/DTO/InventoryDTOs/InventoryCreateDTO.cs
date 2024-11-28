using System.Text.Json.Serialization;

namespace BeeStore_Repository.DTO.InventoryDTOs
{
    public class InventoryCreateDTO
    {

        //public ulong? IsDeleted { get; set; }
        public string Name { get; set; }
        public decimal? MaxWeight { get; set; }

        public decimal? Weight { get; set; } = 0;
        public decimal? Price { get; set; }

        [JsonIgnore]
        public DateTime? BoughtDate { get; set; }

        [JsonIgnore]
        public DateTime? ExpirationDate { get; set; }

        public int WarehouseId { get; set; }

    }
}
