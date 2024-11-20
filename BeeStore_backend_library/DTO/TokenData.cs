namespace BeeStore_Repository.DTO
{
    public class TokenData
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}
