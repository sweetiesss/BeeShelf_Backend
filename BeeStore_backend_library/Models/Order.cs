using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class Order : BaseEntity
{
    //public int Id { get; set; }

    public string? PartnerEmail { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? OrderStatus { get; set; }

    public string? CancellationReason { get; set; }

    public string? ReceiverPhone { get; set; }

    public string? ReceiverAddress { get; set; }

    public int? ProductAmount { get; set; }

    public decimal? TotalPrice { get; set; }

    public string? CodStatus { get; set; }

    public DateTime? DeliverBy { get; set; }

    public int? PictureId { get; set; }

    public int? ProductId { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual User? PartnerEmailNavigation { get; set; }

    public virtual Picture? Picture { get; set; }

    public virtual Product? Product { get; set; }
}
