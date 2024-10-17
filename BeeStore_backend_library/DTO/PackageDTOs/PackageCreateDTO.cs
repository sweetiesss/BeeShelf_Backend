namespace BeeStore_Repository.DTO.PackageDTOs
{
    public class PackageCreateDTO
    {
        public int? Amount { get; set; }
        public int? ProductId { get; set; }
        public int? ProductAmount { get; set; }
        public int? InventoryId { get; set; }
    }
}
