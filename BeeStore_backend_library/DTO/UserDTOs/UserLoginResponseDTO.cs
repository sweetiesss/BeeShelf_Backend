namespace BeeStore_Repository.DTO.UserDTOs
{
    public class UserLoginResponseDTO
    {
        public UserLoginResponseDTO(string email, string role)
        {
            this.email = email;
            this.role = role;
        }

        public string email { get; set; }
        public string role { get; set; }
    }
}
