namespace BeeStore_Repository.DTO.WarehouseShipperDTOs
{
    public class WarehouseShipperListDTO
    {

        public int? UserId { get; set; }
        public string user_email { get; set; }
        public string? Status { get; set; }
        public int? WarehouseId { get; set; }
        public string WarehouseName { get; set; }

    }
}
