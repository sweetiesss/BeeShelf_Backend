using BeeStore_Repository.DTO.PackageDTOs;

namespace BeeStore_Repository.DTO.InventoryDTOs
{
    public class RoomLotListDTO
    {
        public int Id { get; set; }

        public int? OcopPartnerId { get; set; }

        public string? RoomCode { get; set; }

        public decimal? MaxWeight { get; set; }

        public decimal? Weight { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }
        public decimal? X { get; set; }
        public decimal? Y { get; set; }

        public DateTime? BoughtDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public int? StoreId {  get; set; }
        public string StoreName { get; set; }

        public List<LotListDTO> Lots { get; set; }
    }
}
