using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class Warehouse
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public float? Size { get; set; }

    public string? Location { get; set; }

    public DateTime? CreateDate { get; set; }

    public ulong? IsDeleted { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<WarehouseShipper> WarehouseShippers { get; set; } = new List<WarehouseShipper>();

    public virtual ICollection<WarehouseStaff> WarehouseStaffs { get; set; } = new List<WarehouseStaff>();
}
