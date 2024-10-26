using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class WarehouseStaff : BaseEntity
{
    //public int Id { get; set; }

    public int? EmployeeId { get; set; }

    public int? WarehouseId { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Warehouse? Warehouse { get; set; }
}
