namespace BeeStore_Repository.Models;

public partial class Vehicle : BaseEntity
{
    //public int Id { get; set; }

    public string? Name { get; set; }

    public string? LicensePlate { get; set; }

    public decimal? Capacity { get; set; }

    public string? Type { get; set; }

    public ulong? IsCold { get; set; }

    public string? Status { get; set; }

    public int? AssignedDriverId { get; set; }

    public int? StoreId { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual Employee? AssignedDriver { get; set; }

    public virtual Store? Store { get; set; }
}
