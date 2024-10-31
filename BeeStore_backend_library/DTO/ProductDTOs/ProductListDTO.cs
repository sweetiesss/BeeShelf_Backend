namespace BeeStore_Repository.DTO.ProductDTOs
{
    public class ProductListDTO
    {

        public int Id { get; set; }

        public string? OcopPartnerEmail {  get; set; }

        public string? Barcode { get; set; }

        public string? Name { get; set; }

        public decimal? Price { get; set; }

        public decimal? Weight { get; set; }

        public DateTime? CreateDate { get; set; }

        public int? ProductCategoryId { get; set; }

        public string? ProductCategoryName { get; set; }

        public string? PictureLink { get; set; }

        public string? Origin { get; set; }


    }
}
