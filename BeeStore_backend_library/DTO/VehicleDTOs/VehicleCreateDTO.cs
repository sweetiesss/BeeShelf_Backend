using System.Text.Json.Serialization;

namespace BeeStore_Repository.DTO.VehicleDTOs
{
    public class VehicleCreateDTO
    {

        public string? Name { get; set; }

        public string? LicensePlate { get; set; }

        public int? StoreId { get; set; }
        public ulong? IsCold { get; set; }

        public decimal? Capacity { get; set; }

        [JsonIgnore]
        public string? Type { get; set; }

        [JsonIgnore]
        public string? Status { get; set; }

    }
}
