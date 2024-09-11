﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO
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

        public string? Picture { get; set; }

        public bool? IsDeleted { get; set; }

        public string RoleName { get; set; }

    }
}
