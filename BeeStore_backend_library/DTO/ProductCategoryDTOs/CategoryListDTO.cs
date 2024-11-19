namespace BeeStore_Repository.DTO.ProductCategoryDTOs
{
    public class CategoryListDTO
    {
        public int Id { get; set; }

        public string? Type { get; set; }

        public int? OcopCategoryId { get; set; }

        public ulong? IsDeleted { get; set; }

    }
}
