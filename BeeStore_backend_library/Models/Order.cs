using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class Order
{
    public int Id { get; set; }

    public string? PartnerEmail { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? OrderProcessStatus { get; set; }

    public string? ReceiverAddress { get; set; }

    public string? ReceiverPhone { get; set; }

    public decimal? ProductPrice { get; set; }

    public int? ProductAmount { get; set; }

    public decimal? TotalPrice { get; set; }

    public bool? IsCod { get; set; }

    public string? OrderDeliveryStatus { get; set; }

    public string? DeliverBy { get; set; }

    public int? ProductId { get; set; }

    public ulong? IsDeleted { get; set; }

    public virtual Partner? PartnerEmailNavigation { get; set; }

    public virtual Product? Product { get; set; }
}
