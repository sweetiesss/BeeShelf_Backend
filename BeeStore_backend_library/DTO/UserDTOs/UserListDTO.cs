using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

    }
}
