namespace BeeStore_Repository.DTO.ProductCategoryDTOs
{
    public class ProductCategoryListDTO
    {
        public int Id { get; set; }
        public string? TypeName { get; set; }

        public string? TypeDescription { get; set; }
        public int? CategoryId { get; set; }

        public int? ExpireIn { get; set; }
    }
}
