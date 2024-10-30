using System.Text.Json.Serialization;

namespace BeeStore_Repository.DTO.PackageDTOs
{
    public class LotCreateDTO
    {

        public string? LotNumber { get; set; }

        public string? Name { get; set; }

        [JsonIgnore]
        public DateTime? CreateDate { get; set; } = DateTime.Now;

        public int? Amount { get; set; }

        public int? ProductId { get; set; }

        public int? ProductAmount { get; set; }

        [JsonIgnore]
        public int? InventoryId { get; set; }
    }
}
