namespace BeeStore_Repository.DTO.UserDTOs
{
    public class UserLoginRequestDTO
    {
        public required string email { get; set; }
        public required string password { get; set; }
    }
}
