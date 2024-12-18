using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO.UserDTOs
{
    public class ManagerDashboardDTO
    {
        public int? totalWarehouse {  get; set; }
        public int? totalInventory { get; set; }
        public int? totalVehicle { get; set; }
        public int? totalEmployee { get; set; }
        public int? totalStaff {  get; set; }
        public int? totalShipper { get; set; }
        public List<WarehouseRevenueDTO> data { get; set; } = new List<WarehouseRevenueDTO>();
    }
    public class WarehouseRevenueDTO
    {
        public int? WarehouseId { get; set; }
        public string? name { get; set; }
        public string? location { get; set; }
        public ulong? isCold {  get; set; }
        public decimal? totalRevenue { get; set; }
        public int? totalInventoryRevenue { get; set; }
        public int? totalBoughtInventory {  get; set; }
        public int? totalUnboughtInventory { get; set; }
    }
}
