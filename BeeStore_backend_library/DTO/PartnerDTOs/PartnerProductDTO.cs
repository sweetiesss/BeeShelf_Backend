namespace BeeStore_Repository.DTO.PartnerDTOs
{
    public class PartnerProductDTO
    {
        public int totalProductAmount { get; set; }
        public List<ProductDTO> Products { get; set; }

    }

    public class ProductDTO
    {
        public int id { get; set; }
        public string ProductName { get; set; }
        public int stock { get; set; }
        public int warehouseId { get; set; }
        public string warehouseName { get; set; }
        public string warehouseLocation { get; set; }
    }
}
