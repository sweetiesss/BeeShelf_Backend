using System.Text.Json.Serialization;

namespace BeeStore_Repository.DTO.WarehouseDTOs
{
    public class StoreCreateDTO
    {

        public string? Name { get; set; }

        public int? Capacity { get; set; }

        public string? Location { get; set; }
        public int? ProvinceId { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }
        public int? Cols { get; set; }

        [JsonIgnore]
        public DateTime? CreateDate { get; set; }


    }

    public class DeliveryZoneCreateDTO
    {
        public string? Name { get; set; }

        public int? ProvinceId { get; set; }
    }
}
