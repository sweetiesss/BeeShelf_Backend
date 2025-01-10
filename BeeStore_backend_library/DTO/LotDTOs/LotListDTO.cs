namespace BeeStore_Repository.DTO.PackageDTOs
{
    public class LotListDTO
    {
        public int Id { get; set; }

        public string? LotNumber { get; set; }

        public string? Name { get; set; }

        public DateTime? CreateDate { get; set; }

        public int? LotAmount { get; set; }

        public int ProductId { get; set; }
        public string? ProductUnit {  get; set; }
        public decimal? ProductWeight { get; set; }
        public string? ProductName { get; set; }
        public string? ProductPictureLink { get; set; }

        public int? StoreId { get; set; }
        public string? StoreName { get; set; }
        public bool? isCold { get; set; }

        public int? ProductPerLot { get; set; }

        public int? TotalProductAmount { get; set; }

        public DateTime? ImportDate { get; set; }

        public DateTime? ExportDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public int? RoomId { get; set; }
        public string? RoomCode { get; set; }

    }
}
