using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class WarehouseShipper
{
    public int Id { get; set; }

    public bool? Status { get; set; }

    public string UserEmail { get; set; } = null!;

    public int? WarehouseId { get; set; }

    public virtual User UserEmailNavigation { get; set; } = null!;

    public virtual Warehouse? Warehouse { get; set; }
}
