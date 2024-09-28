using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO.InventoryDTOs
{
    public class InventoryListDTO
    {
        public int Id { get; set; }

        public string? PartnerEmail { get; set; }

        public decimal? MaxWeight { get; set; }

        public decimal? Weight { get; set; }

        public DateTime? BoughtDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public int? WarehouseId { get; set; }

        //public ulong? IsDeleted { get; set; }
    }
}
