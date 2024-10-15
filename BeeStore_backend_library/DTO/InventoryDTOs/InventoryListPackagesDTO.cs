using BeeStore_Repository.DTO.PackageDTOs;
using BeeStore_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO.InventoryDTOs
{
    public class InventoryListPackagesDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? UserId { get; set; }

        public decimal? MaxWeight { get; set; }

        public decimal? Weight { get; set; }

        public DateTime? BoughtDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public string WarehouseName { get; set; }

        public List<PackageListDTO> Packages { get; set; }
    }
}
