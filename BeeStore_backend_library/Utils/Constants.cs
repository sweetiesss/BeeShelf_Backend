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
            public const string systemJsonFile = "appsettings.json";
            public const string String = "string";
        }

        public static class Smtp
        {
            public const int DEFAULT_PASSWORD_LENGTH = 12;
            public const string lowercaseLetters = "abcdefghijklmnopqrstuvwxyz";
            public const string uppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            public const string digits = "0123456789";
            public const string specialCharacters = "!@#$%^&*()_+-=[]{}|;:',.<>?/";
            public const string characterSet = lowercaseLetters + uppercaseLetters + digits + specialCharacters;
            public const string smtp = "smtp.gmail.com";

            public const string registerMailSubject = "BeeShelf Account Password";
        }
    }
}
