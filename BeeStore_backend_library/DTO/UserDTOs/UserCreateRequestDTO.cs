using BeeStore_Repository.DTO.UserDTOs.Interfaces;
using System.Text.Json.Serialization;

namespace BeeStore_Repository.DTO.UserDTOs
{
    public class UserCreateRequestDTO : IRoleNameProvider
    {

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Phone { get; set; }

        [JsonIgnore]
        public DateTime? CreateDate { get; set; } = DateTime.Now;

        [JsonIgnore]
        public int? PictureId { get; set; } = 1;

        [JsonIgnore]
        public bool? IsDeleted { get; set; } = false;

        public string RoleName { get; set; }
    }
}
