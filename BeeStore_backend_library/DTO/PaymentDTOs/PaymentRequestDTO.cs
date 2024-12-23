namespace BeeStore_Repository.DTO.PaymentDTOs
{
    public class PaymentRequestDTO
    {
        public string BuyerEmail { get; set; }
        public string CancelUrl { get; set; }
        public string ReturnUrl { get; set; }
        public string Description { get; set; }
    }
}
