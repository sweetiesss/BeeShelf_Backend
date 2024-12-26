using System.Text.Json.Serialization;

namespace BeeStore_Repository.DTO.UserDTOs
{
    public class UserSignUpRequestDTO
    {
        public string Email { get; set; } = null!;

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

        public string? PictureLink { get; set; }

        [JsonIgnore]
        public int? RoleId { get; set; }


        public int? ProvinceId { get; set; }


        public int? CategoryId { get; set; }


        public int? OcopCategoryId { get; set; }

        [JsonIgnore]
        public DateTime? CreateDate { get; set; } = DateTime.Now;

        [JsonIgnore]
        public bool? IsDeleted { get; set; } = false;

    }
}
