using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class DeliveryZone : BaseEntity
{
    //public int Id { get; set; }

    public string? Name { get; set; }

    public string? Location { get; set; }

    public int? WarehouseId { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual ICollection<Batch> Batches { get; set; } = new List<Batch>();

    public virtual Warehouse? Warehouse { get; set; }

    public virtual ICollection<WarehouseShipper> WarehouseShippers { get; set; } = new List<WarehouseShipper>();
}
