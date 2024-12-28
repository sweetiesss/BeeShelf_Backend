namespace BeeStore_Repository.DTO.WarehouseDTOs
{
    public class StoreDeliveryZoneDTO
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public int? Capacity { get; set; }
        public ulong? IsCold { get; set; }
        public int? ProvinceId { get; set; }
        public string? ProvinceName { get; set; }
        public string? Location { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }
        public int? Cols { get; set; }
        public DateTime? CreateDate { get; set; }
        public List<DeliveryZoneDTO> DeliveryZones { get; set; }
    }

    public class DeliveryZoneDTO
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public int? ProvinceId { get; set; }

        public string? ProvinceName { get; set; }

    }
}
