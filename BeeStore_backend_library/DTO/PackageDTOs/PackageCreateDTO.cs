using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO.PackageDTOs
{
    public class PackageCreateDTO
    {
        public decimal? Weight { get; set; }
        public int? Amount { get; set; }
        public int? ProductId { get; set; }
        public int? ProductAmount { get; set; }
        public int? InventoryId { get; set; }
    }
}
