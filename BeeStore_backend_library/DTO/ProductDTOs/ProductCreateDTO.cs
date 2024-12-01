using System.Text.Json.Serialization;

namespace BeeStore_Repository.DTO.ProductDTOs
{
    public class ProductCreateDTO
    {
        public int? OcopPartnerId { get; set; }

        public string? Barcode { get; set; }

        public string? Name { get; set; }

        public decimal? Price { get; set; }

        public decimal? Weight { get; set; }
        public string? Unit { get; set; }

        public ulong? IsCold { get; set; } = 0;

        [JsonIgnore]
        public DateTime? CreateDate { get; set; } = DateTime.Now;

        public int? ProductCategoryId { get; set; }

        public string? PictureLink { get; set; }

        public string? Origin { get; set; }

    }
}
