using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO.InventoryDTOs
{
    public class InventoryCreateDTO
    {
        public string Name { get; set; }
        public decimal? MaxWeight { get; set; }

        public decimal? Weight { get; set; }

        [JsonIgnore]
        public DateTime? BoughtDate { get; set; }

        [JsonIgnore]
        public DateTime? ExpirationDate { get; set; }

        public int WarehouseId { get; set; }

    }
}
