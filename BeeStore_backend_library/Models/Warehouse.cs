namespace BeeStore_Repository.Models;

public partial class Warehouse : BaseEntity
{
    //public int Id { get; set; }

    public string? Name { get; set; }

    public decimal? Capacity { get; set; }

    public string? Location { get; set; }

    public int? ProvinceId { get; set; }

    public string? Type { get; set; }

    public DateTime? CreateDate { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual Province? Province { get; set; }

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

    public virtual ICollection<WarehouseShipper> WarehouseShippers { get; set; } = new List<WarehouseShipper>();

    public virtual ICollection<WarehouseStaff> WarehouseStaffs { get; set; } = new List<WarehouseStaff>();
}
