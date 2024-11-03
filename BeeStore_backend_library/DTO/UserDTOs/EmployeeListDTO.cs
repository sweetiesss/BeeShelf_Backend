namespace BeeStore_Repository.DTO.UserDTOs
{
    public class EmployeeListDTO
    {
        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Phone { get; set; }

        public string? Status { get; set; }

        public DateTime? CreateDate { get; set; }

        public ulong? IsDeleted { get; set; }

        public string? Setting { get; set; }

        public string? PictureLink { get; set; }

        public string RoleName { get; set; }

        public int? WorkAtWarehouseId { get; set; }
        public string? WorkAtWarehouseName { get; set; }


    }
}
