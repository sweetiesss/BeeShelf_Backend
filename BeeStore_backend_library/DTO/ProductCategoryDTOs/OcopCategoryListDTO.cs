using BeeStore_Repository.Models;

namespace BeeStore_Repository.DTO.ProductCategoryDTOs
{
    public class OcopCategoryListDTO
    {
        public int Id { get; set; }

        public string? Type { get; set; }

        public ulong? IsDeleted { get; set; }

        public  List<CategoryListDTO> Categories { get; set; } = new List<CategoryListDTO>();
    }
}
