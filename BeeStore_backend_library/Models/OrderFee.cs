using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class OrderFee : BaseEntity
{
    //public int Id { get; set; }

    public int? OrderId { get; set; }

    public decimal? DeliveryFee { get; set; }

    public decimal? StorageFee { get; set; }

    public decimal? AdditionalFee { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual Order? Order { get; set; }
}
