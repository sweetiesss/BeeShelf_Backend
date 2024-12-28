namespace BeeStore_Repository.DTO.WarehouseDTOs
{
    public class StoreListDTO
    {

        public int Id { get; set; }

        public string? Name { get; set; }

        public int? Capacity { get; set; }
        public int? AvailableCapacity { get; set; }
        public int? UnboughtInventory { get; set; }
        public int? TotalInventory { get; set; }
        public int? ProvinceId { get; set; }
        public string? ProvinceName { get; set; }
        public string? Location { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }
        public int? Cols {  get; set; }

        public DateTime? CreateDate { get; set; }
    }
}
