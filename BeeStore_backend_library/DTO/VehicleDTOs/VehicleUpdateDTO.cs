using System.Text.Json.Serialization;

namespace BeeStore_Repository.DTO.VehicleDTOs
{
    public class VehicleUpdateDTO
    {
        public string? Name { get; set; }

        public string? LicensePlate { get; set; }
        public ulong? IsCold { get; set; }

        [JsonIgnore]
        public decimal? Capacity { get; set; }

        [JsonIgnore]
        public string? Type { get; set; }

        [JsonIgnore]
        public string? Status { get; set; }
    }
}
