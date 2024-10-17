namespace BeeStore_Repository.DTO.PartnerDTOs
{
    public class PartnerListDTO
    {
        public int Id { get; set; }

        public string? CitizenIdentificationNumber { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string? BankName { get; set; }

        public string? BankAccountNumber { get; set; }

        //public int? UserId { get; set; }
        public string User_Email { get; set; }
    }
}
