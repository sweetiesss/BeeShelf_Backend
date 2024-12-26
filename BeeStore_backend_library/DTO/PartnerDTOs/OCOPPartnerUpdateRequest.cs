namespace BeeStore_Repository.DTO.PartnerDTOs
{
    public class OCOPPartnerUpdateRequest
    {
        public string Email { get; set; } = null!;

        //public string ConfirmPassword { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Phone { get; set; }

        public string? TaxIdentificationNumber { get; set; }

        public string? BusinessName { get; set; }

        public string? BusinessNameInternational { get; set; }

        public string? BusinessShortName { get; set; }

        public string? BusinessAddress { get; set; }

        public string? BankName { get; set; }

        public string? BankAccountNumber { get; set; }

        public string? Setting { get; set; }

        public string? PictureLink { get; set; }

        public int? ProvinceId { get; set; }

        public int? CategoryId { get; set; }

        public int? OcopCategoryId { get; set; }
    }
}
