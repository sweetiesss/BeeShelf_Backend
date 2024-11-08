using Org.BouncyCastle.Tls;

namespace BeeStore_Repository.Utils
{
    public static class ResponseMessage
    {
        //General message
        public const string Success = "Success.";
        public const string InvalidPageSize = "Page size is invalid.";
        public const string BadRequest = "Bad request.";

        //JWT response message
        public const string JWTSecretKeyError = "Key Vault JWT Secret Key values are missing.";
        public const string JWTIssuerValueError = "Key Vault JWT Issuer values are missing.";
        public const string JwtAudienceValueError = "Key Vault JWT Audience values are missing.";
        public const string JwtTokenHasNotExpired = "Token has not expired yet.";

        //Picture response message
        public const string PictureUploadImageException = "Something wrong while attempting to upload picture.";
        public const string ImageIdNotFound = "Image not found.";

        //Vehicle Response Message
        public const string VehicleIdNotFound = "Vehicle not found.";
        public const string VehicleLicensePlateDuplicate = "License Plate already exist.";

        //Transaction response message
        public const string TransactionNotFound = "Transaction not found.";

        //Role responise message
        public const string RoleNotFound = "Role not found.";

        //User response message
        public const string UserIdNotFound = "User Id not found.";
        public const string UserEmailNotFound = "User email not found.";
        public const string UserEmailDuplicate = "User email already exist.";
        public const string UserPasswordError = "Password is incorrect.";
        public const string UserRoleError = "This user's role is already higher or equal to Partner.";
        public const string UserRoleNotPartnerError = "This user is not a partner.";
        public const string UserRoleNotShipperError = "This user is not a shipper.";
        public const string UserRoleNotStaffError = "This user is not a staff.";
        public const string UserMismatch = "User mismatched.";
        public const string UpdatePartnerError = "You have to wait for 30 days until you can update again.";

        //Partner response message
        public const string PartnerIdNotFound = "Partner Id not found.";

        //Wallet response message
        public const string WalletIdNotFound = "Wallet Id not found.";

        //Inventory response message
        public const string InventoryIdNotFound = "Inventory Id not found.";
        public const string InventoryOccupied = "Inventory is already occupied.";
        public const string InventoryOverWeightError = "Inventory will exceed the max weight if this item is added in.";
        public const string InventoryNameDuplicate = "Inventory Name already exist.";
        public const string InventoryPartnerNotMatch = "This Inventory does not belongs to this user.";

        //Warehouse response message
        public const string WarehouseIdNotFound = "Warehouse Id not found.";
        public const string WarehouseNameDuplicate = "Warehouse with this name already exist.";
        public const string WarehouseUserDuplicateList = "Please check provided list. The following user is duplicate: ";
        public const string WarehouseUserAddListFailed = "Failed to add. These user are already working at a warehouse ";


        //Product response message
        public const string ProductIdNotFound = "Product Id not found.";
        public const string ProductNameDuplicate = "Failed to add. Product name already exist. ";
        public const string ProductListDuplicate = "Please check your provided list, these name are duplicate: ";
        public const string ProductPartnerNotMatch = "This product does not belongs to this user.";

        //Package response message
        public const string PackageIdNotFound = "Package Id not found.";

        //Order response message
        public const string OrderIdNotFound = "Order Id not found.";
        public const string OrderProccessedError = "You can't edit proccessed order.";
        public const string OrderCanceledError = "You can't canceled finished order.";
        public const string OrderSentError = "Order is already sent.";
        public const string OrderDetailsError = "All items must be from the same warehouse.";
        public const string OrderBatchError = "There are orders that has already been assign to a batch.";

        //Product category response message
        public const string ProductCategoryIdNotFound = "Product Category Id not found.";
        public const string ProductCategoryDuplicate = "This product category already exist.";

        //Request response message
        public const string RequestIdNotFound = "Request Id not found.";
        public const string RequestStatusError = "Request has already been proccessed.";

        //Warehouse category response message
        public const string WarehouseCategoryAddListFailed = "Failed to add. Warehouse with these category already exist: ";
        public const string WarehouseCategoryDuplicateList = "Please check provided list. The following category is duplicate: ";


    }
}
