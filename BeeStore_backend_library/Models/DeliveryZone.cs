namespace BeeStore_Repository.Models;

public partial class DeliveryZone : BaseEntity
{
    //public int Id { get; set; }

    public string? Name { get; set; }

    public int? ProvinceId { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual ICollection<Batch> Batches { get; set; } = new List<Batch>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Province? Province { get; set; }

    public virtual ICollection<WarehouseShipper> WarehouseShippers { get; set; } = new List<WarehouseShipper>();
}
