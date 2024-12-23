using BeeStore_Repository.DTO.InventoryDTOs;

namespace BeeStore_Repository.DTO.WarehouseDTOs
{
    public class StoreListInventoryDTO
    {

        public int Id { get; set; }

        public string? Name { get; set; }

        public int? Capacity { get; set; }
        public ulong? IsCold { get; set; }
        public int? ProvinceId { get; set; }
        public string? ProvinceName { get; set; }
        public string? Location { get; set; }
        public int TotalInventory { get; set; }
        public DateTime? CreateDate { get; set; }
        public List<RoomListDTO> Inventories { get; set; }
    }

}

