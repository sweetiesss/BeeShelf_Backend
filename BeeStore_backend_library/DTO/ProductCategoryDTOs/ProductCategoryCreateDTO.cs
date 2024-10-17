namespace BeeStore_Repository.DTO.ProductCategoryDTOs
{
    public class ProductCategoryCreateDTO
    {
        public string? TypeName { get; set; }

        public string? TypeDescription { get; set; }

        public int? ExpireIn { get; set; }
    }
}
