namespace BeeStore_Repository.DTO.UserDTOs
{
    public class UserListDTO
    {
        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Phone { get; set; }

        public DateTime? CreateDate { get; set; }

        public int? PictureId { get; set; }
        public string? Picture_Link { get; set; }

        public bool? IsDeleted { get; set; }

        public string RoleName { get; set; }

        public string? BankAccountNumber { get; set; }
        public string? BankName { get; set; }

    }
}
