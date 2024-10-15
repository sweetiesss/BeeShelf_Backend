using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Utils
{
    public static class Constants
    {
        public static class RoleName
        {
            public const string Admin = "Admin";
            public const string Manager = "Manager";
            public const string Staff = "Staff";
            public const string User = "User";
            public const string Shipper = "Shipper";
            public const string Partner = "Partner";
        }

        public static class RequestStatus
        {
            public const string Pending = "Pending";
            public const string Approved = "Approved";
            public const string Reject = "Reject";
        }
    }
}
