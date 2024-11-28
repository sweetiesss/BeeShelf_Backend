namespace BeeStore_Repository.DTO.InventoryDTOs
{
    public class InventoryUpdateDTO
    {
        public int Id { get; set; }
        public decimal? Price { get; set; }
        public string Name { get; set; }
        public int? MaxWeight { get; set; }
        public int? Weight { get; set; }

    }
}
