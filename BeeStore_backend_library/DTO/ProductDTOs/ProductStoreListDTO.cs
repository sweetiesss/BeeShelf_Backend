namespace BeeStore_Repository.DTO.ProductDTOs
{
    public class ProductStoreListDTO
    {
        public int totalProduct {  get; set; }
        public List<StoreDTO> data { get; set; }
    }

    public class StoreDTO
    {
        public int Id { get; set; }
        public int? ProvinceId { get; set; }
        public string? ProvinceName { get; set; }
        public string? Location { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public int? ProductInStorage { get; set; }
    }
}
