namespace BeeStore_Repository.DTO.PartnerDTOs
{
    public class PartnerRevenueDTO
    {
        public int? Month { get; set; }
        public List<RevenueDTO> data { get; set; } = new List<RevenueDTO>();
    }

    public class RevenueDTO
    {
        public string? orderStatus { get; set; }
        public int orderAmount { get; set; } = 0;
        public int amount { get; set; } = 0;
    }
}
