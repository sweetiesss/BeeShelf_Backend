using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO.UserDTOs
{
    public class UserCreateRequestDTO
    {

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Phone { get; set; }

        public DateTime? CreateDate { get; set; }

        public int? PictureId { get; set; }

        [JsonIgnore]
        public bool? IsDeleted { get; set; } = false;

        public string RoleName { get; set; }
    }
}
