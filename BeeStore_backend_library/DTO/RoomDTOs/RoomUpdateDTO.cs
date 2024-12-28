namespace BeeStore_Repository.DTO.InventoryDTOs
{
    public class RoomUpdateDTO
    {
        public int Id { get; set; }
        public decimal? Price { get; set; }
        public string RoomCode { get; set; }
        public int? MaxWeight { get; set; }
        public int? Weight { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }
        public decimal? X { get; set; }
        public decimal? Y { get; set; }
        public ulong? IsCold {  get; set; }
    }
}
