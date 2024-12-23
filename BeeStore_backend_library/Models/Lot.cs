namespace BeeStore_Repository.Models;

public partial class Lot : BaseEntity
{
    //public int Id { get; set; }

    public string? LotNumber { get; set; }

    public string? Name { get; set; }

    public DateTime? CreateDate { get; set; }

    public int? LotAmount { get; set; }

    public int? ProductId { get; set; }

    public int? ProductPerLot { get; set; }

    public int? TotalProductAmount { get; set; }

    public DateTime? ImportDate { get; set; }

    public DateTime? ExportDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public int? RoomId { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Product? Product { get; set; }

    public virtual ICollection<Request> RequestExportFromLots { get; set; } = new List<Request>();

    public virtual ICollection<Request> RequestLots { get; set; } = new List<Request>();

    public virtual Room? Room { get; set; }
}
