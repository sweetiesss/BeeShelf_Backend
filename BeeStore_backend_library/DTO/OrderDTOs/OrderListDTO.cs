namespace BeeStore_Repository.DTO.OrderDTOs
{
    public class OrderListDTO
    {
        public int Id { get; set; }
        //public int? UserId { get; set; }
        public string user_email { get; set; }
        public DateTime? CreateDate { get; set; }

        public string? OrderStatus { get; set; }

        public string? CancellationReason { get; set; }

        public string? ReceiverPhone { get; set; }

        public string? ReceiverAddress { get; set; }

        public int? ProductAmount { get; set; }

        public decimal? TotalPrice { get; set; }

        public string? CodStatus { get; set; }

        //public int? DeliverBy { get; set; }
        public string deliver_by { get; set; }

        //public int? PictureId { get; set; }
        public string picture_link { get; set; }

        //public int? ProductId { get; set; }
        public string product_name { get; set; }
    }
}
