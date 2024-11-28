namespace BeeStore_Repository.DTO.InventoryDTOs
{
    public class InventoryListDTO
    {
        public int Id { get; set; }

        public int? OcopPartnerId { get; set; }

        public string? Name { get; set; }

        public decimal? MaxWeight { get; set; }
        public int totalProduct { get; set; }
        public decimal? Price { get; set; }
        public decimal? Weight { get; set; }

        public DateTime? BoughtDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public int? WarehouseId { get; set; }
        public string WarehouseName { get; set; }
    }
}
