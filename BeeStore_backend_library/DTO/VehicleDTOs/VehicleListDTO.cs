using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO.VehicleDTOs
{
    public class VehicleListDTO
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? LicensePlate { get; set; }

        public int? Capacity { get; set; }

        public string? Type { get; set; }

        public string? Status { get; set; }

        public int? AssignedDriverId { get; set; }
        public string? AssignedDriverEmail {  get; set; }
        public string? AssignedDriverName { get; set; }
    }
}
