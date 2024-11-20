namespace BeeStore_Repository.DTO.UserDTOs
{
    public class UserForgotPasswordRequest
    {
        public string token { get; set; }
        public string newPassword { get; set; }
    }
}
