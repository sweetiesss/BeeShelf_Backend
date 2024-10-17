namespace BeeStore_Repository.DTO.RequestDTOs
{
    public class RequestListDTO
    {
        public int Id { get; set; }
        public string user_email { get; set; }

        public string? Description { get; set; }

        public int? PackageId { get; set; }
        public string ProductName { get; set; }

        public int? SendToInventory { get; set; }

        public DateTime? CreateDate { get; set; }

        public string? Status { get; set; }

        public string? RequestType { get; set; }
    }
}
