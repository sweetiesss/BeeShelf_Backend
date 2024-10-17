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

        public static class Status
        {
            public const string Pending = "Pending";
            public const string Approved = "Approved";
            public const string Reject = "Reject";
            public const string Processing = "Processing";
            public const string Shipped = "Shipped";
            public const string Canceled = "Canceled";
        }

        public static class DefaultString
        {
            public const string String = "string";
        }

    }
}
