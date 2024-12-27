namespace BeeStore_Repository.DTO.InventoryDTOs
{
    public class RoomListDTO
    {
        public int Id { get; set; }

        public int? OcopPartnerId { get; set; }

        public string? RoomCode { get; set; }

        public decimal? MaxWeight { get; set; }
        public int totalProduct { get; set; }
        public decimal? Price { get; set; }
        public decimal? Weight { get; set; }

        public DateTime? BoughtDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public int? StoreId { get; set; }
        public string? StoreName { get; set; }
        public string? StoreLocation { get; set; }
        public ulong? IsCold { get; set; }
    }
}
