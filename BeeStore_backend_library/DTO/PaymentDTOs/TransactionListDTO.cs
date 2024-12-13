namespace BeeStore_Repository.DTO.PaymentDTOs
{
    public class TransactionListDTO
    {
        public int Id { get; set; }

        public string? Code { get; set; }

        public int? Amount { get; set; }

        public string? Description { get; set; }

        public int? OcopPartnerId { get; set; }
        public string? Email { get; set; }

        public DateTime? CreateDate { get; set; }

        public string? Status { get; set; }

        public string? CancellationReason { get; set; }
    }
}
