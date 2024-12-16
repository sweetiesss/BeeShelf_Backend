namespace BeeStore_Repository.DTO.RequestDTOs
{
    public class RequestListDTO
    {
        public int Id { get; set; }
        public int? OcopPartnerId { get; set; }
        public string? partner_email { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? LotId { get; set; }
        public int? LotAmount {  get; set; }
        public int? ProductPerLotAmount {  get; set; }
        public int? TotalProductAmount {  get; set; }
        public string? ProductName { get; set; }
        public string? ProductImage { get; set; }
        public string? RequestType { get; set; }
        public int? ExportFromLotId { get; set; }
        public int? SendToInventoryId { get; set; }
        public string? WarehouseName { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ApporveDate { get; set; }
        public DateTime? DeliverDate { get; set; }
        public DateTime? CancelDate { get; set; }
        public string? CancellationReason { get; set; }
        public string? Status { get; set; }
    }
}
