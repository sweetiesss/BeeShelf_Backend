using BeeStore_Repository.DTO.UserDTOs.Interfaces;
using BeeStore_Repository.Utils;
using System.Reflection.Metadata;
using System.Text.Json.Serialization;

namespace BeeStore_Repository.DTO.UserDTOs
{
    public class EmployeeCreateRequest 
    {

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Phone { get; set; }

        [JsonIgnore]
        public string? Status { get; set; } = "Active";

        [JsonIgnore]
        public DateTime? CreateDate { get; set; } = DateTime.Now;

        [JsonIgnore]
        public string? Setting { get; set; }

        [JsonIgnore]
        public string? PictureLink { get; set; } = String.Empty;

        public int? RoleId { get; set; }
    }
}
