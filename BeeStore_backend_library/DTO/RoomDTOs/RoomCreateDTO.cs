using System.Text.Json.Serialization;

namespace BeeStore_Repository.DTO.InventoryDTOs
{
    public class RoomCreateDTO
    {
        public int StoreId { get; set; }
        public List<RoomCreateListDTO> data { get; set; } = new List<RoomCreateListDTO>();
        

    }

    public class RoomCreateListDTO
    {
        public ulong? IsCold { get; set; }
        public string? RoomCode { get; set; }
        public decimal? MaxWeight { get; set; }
        public decimal? Price { get; set; }
        //public int? InventoryAmount { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }
        public decimal? X { get; set; }
        public decimal? Y { get; set; }

        [JsonIgnore]
        public DateTime? BoughtDate { get; set; }

        [JsonIgnore]
        public DateTime? ExpirationDate { get; set; }

    }
}
