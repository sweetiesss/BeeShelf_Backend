using System.Text.Json.Serialization;

namespace BeeStore_Repository.DTO.WarehouseDTOs
{
    public class WarehouseCreateDTO
    {

        public string? Name { get; set; }

        public decimal? Size { get; set; }

        public string? Location { get; set; }

        [JsonIgnore]
        public DateTime? CreateDate { get; set; }
    }
}
