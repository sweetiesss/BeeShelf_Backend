using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO.VehicleDTOs
{
    public class VehicleUpdateDTO
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
