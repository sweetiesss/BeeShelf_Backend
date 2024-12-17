namespace BeeStore_Repository.DTO.UserDTOs
{
    public class ManagerTotalRevenueDTO
    {
        public int? WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public List<MonthRevenueDTO> data { get; set; } = new List<MonthRevenueDTO>();
    }

    public class MonthRevenueDTO
    {
        public int? Month { get; set; }
        public decimal? TotalRevenue { get; set; }
    }
}
