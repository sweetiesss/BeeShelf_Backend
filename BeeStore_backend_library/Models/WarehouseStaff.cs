using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class WarehouseStaff : BaseEntity
{
    //public int Id { get; set; }

    public string? UserEmail { get; set; }

    public int? WarehouseId { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual User? UserEmailNavigation { get; set; }

    public virtual Warehouse? Warehouse { get; set; }
}
