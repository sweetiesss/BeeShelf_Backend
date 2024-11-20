namespace BeeStore_Repository.Models;

public partial class Warehouse : BaseEntity
{
    //public int Id { get; set; }

    public string? Name { get; set; }

    public int? Capacity { get; set; }

    public string? Location { get; set; }

    public DateTime? CreateDate { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual ICollection<DeliveryZone> DeliveryZones { get; set; } = new List<DeliveryZone>();

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

    public virtual ICollection<WarehouseShipper> WarehouseShippers { get; set; } = new List<WarehouseShipper>();

    public virtual ICollection<WarehouseStaff> WarehouseStaffs { get; set; } = new List<WarehouseStaff>();
}
