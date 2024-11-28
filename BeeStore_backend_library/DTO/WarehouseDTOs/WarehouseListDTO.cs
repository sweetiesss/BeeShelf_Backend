namespace BeeStore_Repository.DTO.WarehouseDTOs
{
    public class WarehouseListDTO
    {

        public int Id { get; set; }

        public string? Name { get; set; }

        public int? Capacity { get; set; }
        public int? ProvinceId { get; set; }
        public string? Location { get; set; }

        public string? Type { get; set; }

        public DateTime? CreateDate { get; set; }
    }
}
