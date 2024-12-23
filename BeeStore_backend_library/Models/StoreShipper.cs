namespace BeeStore_Repository.Models;

public partial class StoreShipper : BaseEntity
{
    //public int Id { get; set; }

    public string? Status { get; set; }

    public int? EmployeeId { get; set; }

    public int? DeliveryZoneId { get; set; }

    public int? StoreId { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual DeliveryZone? DeliveryZone { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Store? Store { get; set; }
}
