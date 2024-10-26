namespace BeeStore_Repository.DTO.OrderDTOs
{
    public class OrderListDTO
    {

        public int Id { get; set; }
        public string? partner_email {  get; set; }

        public string? Status { get; set; }

        public string? CancellationReason { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? DeliverStartDate { get; set; }

        public DateTime? DeliverFinishDate { get; set; }

        public DateTime? CompleteDate { get; set; }

        public string? ReceiverPhone { get; set; }

        public string? ReceiverAddress { get; set; }

        public decimal? TotalPrice { get; set; }

        public int? BatchId { get; set; }

        public DateTime? PickDate { get; set; }

        public int? PickStaffId { get; set; }

        public string? PictureLink { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; }
    }

    public class OrderDetailDTO
    {
        public int Id { get; set; }

        public int? LotId { get; set; }
        public string ProductName {  get; set; }

        public int? ProductPrice { get; set; }

        public int? ProductAmount { get; set; }
    }
}
