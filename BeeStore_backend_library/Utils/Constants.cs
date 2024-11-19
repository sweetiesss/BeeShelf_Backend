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

        public static class VehicleType
        {
            public const string Van = "Van";
            public const string Truck = "Truck";
            public const string Motorcycle = "Motorcycle";
        }

        public static class VehicleStatus
        {
            public const string Available = "Available";
            public const string InService = "InService";
            public const string Repair = "Repair";
        }

        public static class PaymentStatus
        {
            public const string Paid = "PAID";
            public const string Pending = "PENDING";
            public const string Processing = "PROCESSING";
            public const string Cancelled = "CANCELLED";
        }

        public static class Status
        {
            public const string Draft = "Draft";
            public const string Pending = "Pending";
            public const string Approved = "Approved";
            public const string Reject = "Reject";
            public const string Processing = "Processing";
            public const string Shipping = "Shipping";
            public const string Delivered = "Delivered";
            public const string Completed = "Completed";
            public const string Returned = "Returned";
            public const string Refunded = "Refunded";
            public const string Canceled = "Canceled";
            public const string Failed = "Failed";

        }

        public static class DefaultString
        {
            public const string systemJsonFile = "appsettings.json";
            public const string String = "string";
        }

        public static class SortCriteria
        {
            //global
            public const string CreateDate = "CreateDate";

            //user
            public const string FirstName = "FirstName";
            public const string LastName = "LastName";
            public const string Email = "Email";

            //package
            public const string Amount = "Amount";
            public const string ProductAmount = "ProductAmount";
            public const string ExpirationDate = "ExpirationDate";

            //order
            public const string TotalPrice = "TotalPrice";

            //inventory
            public const string BoughtDate = "BoughtDate";
            public const string ExiprationDate = "ExpirationDate";
            public const string MaxWeight = "MaxWeight";
            public const string Weight = "Weight";
            public const string Name = "Name";

            //Product
            public const string Price = "Price";
            public const string Origin = "Origin";

            //warehouse
            public const string Size = "Size";
            public const string Location = "Location";

            //Vehicle
            public const string Capacity = "Capacity";
        }

        public static class FilterCriteria
        {
            //package specific
            public const string ProductId = "ProductId";
            public const string InventoryId = "InventoryId";
            public const string Type = "Type";
            public const string Status = "Status";
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
