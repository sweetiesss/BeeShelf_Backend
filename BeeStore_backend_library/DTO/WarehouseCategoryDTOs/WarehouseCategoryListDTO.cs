namespace BeeStore_Repository.DTO.WarehouseCategoryDTOs
{
    public class WarehouseCategoryListDTO
    {
        public int? WarehouseId { get; set; }
        public string warehouse_name { get; set; }
        public int? ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; }
    }
}
