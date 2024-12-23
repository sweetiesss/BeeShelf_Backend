namespace BeeStore_Repository.Models;

public partial class StoreStaff : BaseEntity
{
    //public int Id { get; set; }

    public int? EmployeeId { get; set; }

    public int? StoreId { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Store? Store { get; set; }
}
