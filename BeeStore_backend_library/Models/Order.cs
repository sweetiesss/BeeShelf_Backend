using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class Order : BaseEntity
{
    //public int Id { get; set; }

    public int? OcopPartnerId { get; set; }

    public string? Status { get; set; }

    public string? CancellationReason { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? DeliverStartDate { get; set; }

    public DateTime? DeliverFinishDate { get; set; }

    public DateTime? CompleteDate { get; set; }

    public string? ReceiverPhone { get; set; }

    public string? ReceiverAddress { get; set; }

    public decimal? Distance { get; set; }

    public decimal? TotalPrice { get; set; }

    public int? BatchId { get; set; }

    public DateTime? PickDate { get; set; }

    public int? PickStaffId { get; set; }

    public string? PictureLink { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual Batch? Batch { get; set; }

    public virtual OcopPartner? OcopPartner { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<OrderFee> OrderFees { get; set; } = new List<OrderFee>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Employee? PickStaff { get; set; }
}
