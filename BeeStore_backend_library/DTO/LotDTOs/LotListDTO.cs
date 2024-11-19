namespace BeeStore_Repository.DTO.PackageDTOs
{
    public class LotListDTO
    {

        public int Id { get; set; }

        public string? LotNumber { get; set; }

        public string? Name { get; set; }

        public DateTime? CreateDate { get; set; }

        public int? Amount { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int? ProductAmount { get; set; }

        public DateTime? ImportDate { get; set; }

        public DateTime? ExportDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public int? InventoryId { get; set; }



    }
}
