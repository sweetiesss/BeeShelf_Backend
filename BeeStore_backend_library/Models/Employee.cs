namespace BeeStore_Repository.Models;

public partial class Employee : BaseEntity
{
    //public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Phone { get; set; }

    public string? Status { get; set; }

    public DateTime? CreateDate { get; set; }

    //public ulong? IsDeleted { get; set; }

    public string? Setting { get; set; }

    public string? PictureLink { get; set; }

    public int? RoleId { get; set; }

    public virtual ICollection<Batch> Batches { get; set; } = new List<Batch>();

    public virtual ICollection<MoneyTransfer> MoneyTransfers { get; set; } = new List<MoneyTransfer>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<StoreShipper> StoreShippers { get; set; } = new List<StoreShipper>();

    public virtual ICollection<StoreStaff> StoreStaffs { get; set; } = new List<StoreStaff>();

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
