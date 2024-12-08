namespace BeeStore_Repository.Models;

public partial class Request : BaseEntity
{
    //public int Id { get; set; }

    public int? OcopPartnerId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public int? LotId { get; set; }

    public string? RequestType { get; set; }

    public int? SendToInventoryId { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? ApporveDate { get; set; }

    public DateTime? DeliverDate { get; set; }

    public DateTime? CancelDate { get; set; }

    public string? CancellationReason { get; set; }

    public string? Status { get; set; }

    //public ulong? IsDeleted { get; set; }

    public int? ExportFromLotId { get; set; }

    public virtual Lot? ExportFromLot { get; set; }

    public virtual Lot? Lot { get; set; }

    public virtual OcopPartner? OcopPartner { get; set; }

    public virtual Inventory? SendToInventory { get; set; }
}
