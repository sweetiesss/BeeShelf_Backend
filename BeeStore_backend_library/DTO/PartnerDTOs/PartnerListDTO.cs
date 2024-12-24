namespace BeeStore_Repository.DTO.PartnerDTOs
{
    public class PartnerListDTO
    {

        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Phone { get; set; }

        public string? CitizenIdentificationNumber { get; set; }

        public string? TaxIdentificationNumber { get; set; }

        public string? BusinessName { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string? BankName { get; set; }

        public string? BankAccountNumber { get; set; }

        public string? Setting { get; set; }

        public string? PictureLink { get; set; }

        public int? RoleId { get; set; }

        public string? RoleName { get; set; }
        public int? ProvinceId { get; set; }

        public string? ProvinceCode { get; set; }

        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public int? OcopCategoryId { get; set; }
        public string? OcopCategoryName { get; set; }
        public ulong? IsVerified { get; set; }


    }
}
