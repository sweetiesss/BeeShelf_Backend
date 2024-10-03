using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO.WarehouseShipperDTOs
{
    public class WarehouseShipperCreateDTO
    {
        public int UserId { get; set; }
        public int WarehouseId { get; set; }
        [JsonIgnore]
        public string? Status { get; set; }

    }
}
