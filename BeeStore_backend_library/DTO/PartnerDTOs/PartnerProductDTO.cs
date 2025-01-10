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
        public string ProductImage {  get; set; }
        public int stock { get; set; }
        public int storeId { get; set; }
        public string storeName { get; set; }
        public string storeLocation { get; set; }
    }
}
