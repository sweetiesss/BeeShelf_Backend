namespace BeeStore_Repository.DTO.WarehouseDTOs
{
    public class WarehouseListDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public decimal? Size { get; set; }

        public string? Location { get; set; }

        public DateTime? CreateDate { get; set; }
    }
}
