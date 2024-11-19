namespace BeeStore_Repository.Models;

public partial class Inventory : BaseEntity
{
    //public int Id { get; set; }

    public int? OcopPartnerId { get; set; }

    public string? Name { get; set; }

    public decimal? MaxWeight { get; set; }

    public decimal? Weight { get; set; }

    public DateTime? BoughtDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public int? WarehouseId { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual ICollection<Lot> Lots { get; set; } = new List<Lot>();

    public virtual OcopPartner? OcopPartner { get; set; }

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual Warehouse? Warehouse { get; set; }
}
