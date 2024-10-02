using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class WarehouseShipper : BaseEntity
{

    public string? Status { get; set; }

    public int? UserId { get; set; }

    public int? WarehouseId { get; set; }

    public virtual User? User { get; set; }

    public virtual Warehouse? Warehouse { get; set; }
}
