namespace BeeStore_Repository.DTO.UserDTOs
{
    public class EmployeeUpdateRequest
    {


        public string Email { get; set; } = null!;

        //public string ConfirmPassword { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Phone { get; set; }

        public string? Setting { get; set; }

        public string? PictureLink { get; set; }

    }
}
