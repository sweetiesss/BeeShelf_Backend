namespace BeeStore_Repository.DTO.ProductCategoryDTOs
{
    public class ProductCategoryCreateDTO
    {
        public int? CategoryId { get; set; }

        public string? TypeName { get; set; }

        public string? TypeDescription { get; set; }

        public int? ExpireIn { get; set; }
    }
}
