namespace BeeStore_Repository.DTO.PaymentDTOs
{
    public class ConfirmPaymentDTO
    {
        public string? Code { get; set; }
        public string? Id { get; set; }
        public bool Cancel { get; set; }
        public string? Status { get; set; }
        public string? OrderCode { get; set; }
    }
}
