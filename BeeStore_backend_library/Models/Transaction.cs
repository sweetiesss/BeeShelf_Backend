using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class Transaction : BaseEntity
{

    public string? TransactionCode { get; set; }

    public int? ItemId { get; set; }

    public decimal? Amount { get; set; }

    public string? Description { get; set; }

    public int? UserId { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? Status { get; set; }

    public string? CancellationReason { get; set; }

    public virtual Inventory? Item { get; set; }

    public virtual User? User { get; set; }
}
