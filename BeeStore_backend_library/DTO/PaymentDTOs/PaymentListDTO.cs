namespace BeeStore_Repository.DTO.PaymentDTOs
{
    public class PaymentListDTO
    {
        public int Id { get; set; }

        public int? OcopPartnerId { get; set; }

        public int? OrderId { get; set; }

        public int? CollectedBy { get; set; }
        public string? ShipperEmail { get; set; }
        public string? ShipperName { get; set; }

        public int? TotalAmount { get; set; }

        public ulong? IsConfirmed { get; set; }

        public ulong? IsDeleted { get; set; }


    }
}
