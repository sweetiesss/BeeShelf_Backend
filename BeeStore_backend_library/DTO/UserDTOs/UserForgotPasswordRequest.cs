﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO.UserDTOs
{
    public class UserForgotPasswordRequest
    {
        public string token {  get; set; }
        public string newPassword {  get; set; }
    }
}
