using System.Text.Json.Serialization;

namespace BeeStore_Repository.DTO.VehicleDTOs
{
    public class VehicleCreateDTO
    {

        public string? Name { get; set; }

        public string? LicensePlate { get; set; }

        [JsonIgnore]
        public int? Capacity { get; set; }

        [JsonIgnore]
        public string? Type { get; set; }

        [JsonIgnore]
        public string? Status { get; set; }

    }
}
