namespace BeeStore_Repository.Utils
{
    public static class ResponseMessage
    {
        //token shit im tried of this shit
        public const string InvalidResetToken = "Invalid reset token.";
        public const string ExpiredResetToken = "Reset token expired.";

        //General message
        public const string Success = "Success.";
        public const string Unsupported = "Unsupported Function";
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
        public const string VehicleTypeNotMatchWarehouse = "Vehicle type(cold/warm) not match";
        public const string VehicleCurrentlyInService = "Vehicle is currently in Service";
        public const string VehicleAssigned = "There is still shipper assigned to this vehicle";

        //Transaction response message
        public const string TransactionNotFound = "Transaction not found.";
        public const string TransactionAlreadyProcessed = "Transaction already processed.";

        //Payment
        public const string PaymentNotFound = "Payment has already been processed or doesn't exist.";
        public const string MoneyTransferAlreadyMade = "Money Transfer request has already been processed.";

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
        public const string NotEnoughCredit = "Wallet doesn't have enough money.";

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
        public const string WarehouseAddInventoryCapacityLimitReach = "Warehosue capacity limit reached";


        //Batch response message
        public const string BatchIdNotFound = "Batch Id not found.";
        public const string BatchStatusNotPending = "Can't edit processed batch.";
        public const string BatchAssignedOrder = "Can't assign unprocessed or completed orders.";
        public const string ShipperBatchOrdersOverweight = "Shipper's vehicle doesn't have enough capacity to handle this batch of orders.";
        public const string DeliveryZoneShipperNotMatch = "Delivery Zone of Shipper is not Match with the Delivery Zone of the Batch.";
        public const string DeliveryZoneOrderNotMatch = "Delivery Zone of Order is not Match with the Delivery Zone of the Batch";
        public const string ShipperDeliverColdProduct = "Shipper's assigned vehicle may not match with the type of product.";
        public const string BatchOrderAlreadyShipped = "Can't edit batch that has already processed orders";
        public const string BatchDuplicateName = "There's already a batch with this name";

        //Product response message
        public const string ProductIdNotFound = "Product Id not found.";
        public const string ProductNameDuplicate = "Failed to add. Product name already exist. ";
        public const string ProductListDuplicate = "Please check your provided list, these name are duplicate: ";
        public const string ProductPartnerNotMatch = "This product does not belongs to this user.";
        public const string ProductNotEnough = "Not enough products.";
        public const string ProductMustBeFromTheSameWarehouse = "All product in an order must be in the same warehouse.";
        public const string ProductAndRoomTypeNotMatch = "Product and room type does not match";
        public const string ProductAlreadyInInventory = "Can't edit products already in Inventory.";

        //Package response message
        public const string PackageIdNotFound = "Lot Id not found.";
        public const string NoLotWithProductFound = "No Lot with this Product found.";

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
        public const string RequestDraftCancelError = "Can't cancel draft request.";
        public const string RequestHasNotBeenProcessed = "Request has not been processed or have already been finished.";
        public const string InventoryFromTheSameWarehouse = "You can't export a Lot to the same warehouse.";
        public const string StaffCantProcessedExportOrderFromAnotherWarehouse = "This staff can't processed order export to another warehouse.";

        //Warehouse category response message
        public const string WarehouseCategoryAddListFailed = "Failed to add. Warehouse with these category already exist: ";
        public const string WarehouseCategoryDuplicateList = "Please check provided list. The following category is duplicate: ";

        //Delivery Zone
        public const string DeliveryZoneDuplicateNameOrLocation = "Delivery zone with this name or location has already exist.";
        public const string DeliveryZoneProvinceIdNotMatchWithShipper = "Delivery zone may not be in the same province as the warehouse that shipper is currently working at.";
        public const string DeliveryZoneIdNotFound = "Delivery zone Id not found.";
        //Province
        public const string ProvinceIdNotFound = "Province Id not found.";
        //OcopCategory
        public const string OcopCategoryIdNotFound = "OcopCategory Id not found.";
        //Category
        public const string CategoryIdNotMatch = "Category Id not match with OCOPCategory";
        public const string NoReasonGiven = "No reason was given";

        public const string OcopPartnerVerificationNotFound = "Verification papers not found.";
        public const string RejectVerifiedError = "You can't reject a verified paper.";
    }
}
