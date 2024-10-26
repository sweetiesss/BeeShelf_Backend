using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class Batch : BaseEntity
{
    //public int Id { get; set; }

    public string? Name { get; set; }

    public string? Status { get; set; }

    public DateTime? CompleteDate { get; set; }

    public int? DeliveryZoneId { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual ICollection<BatchDelivery> BatchDeliveries { get; set; } = new List<BatchDelivery>();

    public virtual DeliveryZone? DeliveryZone { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
