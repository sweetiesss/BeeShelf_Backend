using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO.PackageDTOs
{
    public class PackageListDTO
    {
        public int Id { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public decimal? Weight { get; set; }
        public int? Amount { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public int? ProductAmount { get; set; }
        public int? InventoryId { get; set; }




    }
}
