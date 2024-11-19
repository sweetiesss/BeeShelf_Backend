using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO
{
    public class TokenData
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}
