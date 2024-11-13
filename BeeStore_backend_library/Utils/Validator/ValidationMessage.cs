using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Utils.Validator
{
    public static class ValidationMessage
    {
        // Generic validation messages
        public const string PhoneMaxLength = "Phone cannot exceed 11 characters.";
        public const string CitizenIdMaxLength = "Citizen Identification Number cannot exceed 25 characters.";
        public const string TaxIdMaxLength = "Tax Identification Number cannot exceed 25 characters.";
        public const string BusinessNameMaxLength = "Business Name cannot exceed 50 characters.";
        public const string BankNameMaxLength = "Bank Name cannot exceed 25 characters.";
        public const string BankAccountNumberMaxLength = "Bank Account Number cannot exceed 50 characters.";
        public const string EmailInvalidFormat = "Email must be in the format of ...@gmail.com.";

        // Batch specific messages
        public const string NameRequired = "Name cannot be null or empty.";
        public const string NameMaxLength = "Name cannot exceed 25 characters.";
        public const string OrdersNotNull = "Orders list cannot be null.";
        public const string OrdersNotEmpty = "Orders list must contain at least one item.";
        public const string OrderIdGreaterThanZero = "Order Id must be greater than 0 if specified.";

        // Inventory specific messages
        public const string MaxWeightNonNegative = "MaxWeight must be a non-negative value.";
        public const string WeightNonNegative = "Weight must be a non-negative value.";
        public const string WarehouseIdRequired = "WarehouseId is required.";

        // Lot specific messages
        public const string NameMaxLength50 = "Name cannot exceed 50 characters.";
        public const string LotNumberRequired = "LotNumber is required.";
        public const string LotNumberMaxLength = "LotNumber cannot exceed 25 characters.";
        public const string AmountNonNegative = "Amount must be a non-negative value.";
        public const string ProductAmountNonNegative = "ProductAmount must be a non-negative value.";
        public const string ProductIdRqeuired = "ProductId is required.";

        // Order specific messages
        public const string OcopPartnerIdRequired = "OcopPartnerId is required.";
        public const string ReceiverPhoneMaxLength = "ReceiverPhone cannot exceed 11 characters.";
        public const string ReceiverAddressMaxLength = "ReceiverAddress cannot exceed 50 characters.";
        //public const string OrderDetailsNotEmpty = "OrderDetails list must contain at least one item.";
        public const string ProductDetailsNotEmpty = "Products list must contain at least one item.";
        //public const string LotIdRequired = "LotId is required.";
        public const string ProductIdRequired = "ProductId is required.";
        public const string ReceiverPhoneRequired = "ReceiverPhone cannot be null or empty.";
        public const string ReceiverAddressRequired = "ReceiverAddress cannot be null or empty.";

        // OCOPPartner specific messages
        public const string EmailRequired = "Email is required.";
        public const string ConfirmPasswordRequired = "Confirm Password is required.";
        public const string FirstNameRequired = "First Name is required.";
        public const string LastNameRequired = "Last Name is required.";
        public const string PhoneRequired = "Phone is required.";
        public const string CitizenIdRequired = "Citizen Identification Number is required.";
        public const string TaxIdRequired = "Tax Identification Number is required.";
        public const string BusinessNameRequired = "Business Name is required.";
        public const string BankNameRequired = "Bank Name is required.";
        public const string BankAccountNumberRequired = "Bank Account Number is required.";
        public const string ProvinceIdRequired = "ProvinceId is required.";
        public const string CategoryIdRequired = "CategoryId is required.";
        public const string OcopCategoryIdRequired = "OcopCategoryId is required.";
        public const string UserIdRequired = "UserId is required.";


        // Payment specific messages
        public const string BuyerEmailRequired = "Buyer Email is required.";
        public const string CancelUrlRequired = "CancelUrl is required.";
        public const string ReturnUrlRequired = "ReturnUrl is required.";
        public const string DescriptionRequired = "Description is required.";
        public const string CodeRequired = "Code is required.";
        public const string IdRequired = "Id is required.";
        public const string StatusRequired = "Status is required.";
        public const string OrderCodeRequired = "Order Code is required.";

        // ProductCategory specific messages
        public const string TypeNameRequired = "TypeName is required.";
        public const string TypeDescriptionRequired = "TypeDescription is required.";
        public const string ExpireInRequired = "ExpireIn is required.";
        public const string TypeNameMaxLength = "TypeName cannot exceed 50 characters.";
        public const string TypeDescriptionMaxLength = "TypeDescription cannot exceed 100 characters.";

        // Product specific messages
        public const string BarcodeRequired = "Barcode is required.";
        public const string PriceRequired = "Price is required.";
        public const string WeightRequired = "Weight is required.";
        public const string ProductCategoryIdRequired = "ProductCategoryId is required.";
        public const string PictureLinkRequired = "PictureLink is required.";
        public const string OriginRequired = "Origin is required.";
        public const string BarcodeMaxLength = "Barcode cannot exceed 255 characters.";
        public const string OriginMaxLength = "Origin cannot exceed 25 characters.";

        // Request specific messages
        public const string RequestNameRequired = "Name is required.";
        public const string RequestDescriptionRequired = "Description is required.";
        public const string SendToInventoryIdRequired = "SendToInventoryId is required.";
        public const string LotRequired = "Lot is required.";
        public const string RequestTypeRequired = "RequestType is required.";
        public const string CancellationReasonRequired = "CancellationReason is required.";
        public const string RequestNameMaxLength = "Name cannot exceed 25 characters.";
        public const string RequestDescriptionMaxLength = "Description cannot exceed 50 characters.";
        public const string CancellationReasonMaxLength = "CancellationReason cannot exceed 50 characters.";
        public const string RequestTypeMaxLength = "RequestType cannot exceed 10 characters.";
        public const string StatusMaxLength = "Status cannot exceed 10 characters.";

        // User specific messages
        public const string EmailInvalid = "Email is invalid.";
        public const string EmailMaxLength = "Email cannot exceed 25 characters.";
        public const string PasswordRequired = "Password is required.";
        public const string PasswordMinLength = "Password must be at least 8 characters.";
        public const string PasswordMaxLength = "Password cannot exceed 100 characters.";
        public const string PasswordMismatch = "Passwords do not match.";
        public const string FirstNameMaxLength = "First Name cannot exceed 25 characters.";
        public const string LastNameMaxLength = "Last Name cannot exceed 25 characters.";
        public const string RoleIdRequired = "Role Id is required.";
        public const string JwtRequired = "JWT token is required.";

        // Vehicle Validation Messages
        public static string LicensePlateRequired = "License plate is required.";
        public static string LicensePlateMaxLength = "License plate must be no more than 25 characters.";
        public static string CapacityRequired = "Capacity is required.";
        public static string TypeRequired = "Type is required.";
        public static string TypeMaxLength = "Type must be no more than 10 characters.";

        // Warehouse Validation Messages
        public static string LocationRequired = "Location is required.";
        public static string LocationMaxLength = "Location must be no more than 50 characters.";
        public static string CreateDateRequired = "Create date is required.";

        // Warehouse Shipper + Staff Validation Messages
        public static string EmployeeIdRequired = "Employee ID is required.";
    }
}
