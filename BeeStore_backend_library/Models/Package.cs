using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class Package
{
    public int Id { get; set; }

    public DateTime? CreateDate { get; set; }

    public decimal? Weight { get; set; }

    public int? Amount { get; set; }

    public int? ProductId { get; set; }

    public int? ProductAmount { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public int? InventoryId { get; set; }

    public ulong? IsDeleted { get; set; }

    public virtual Inventory? Inventory { get; set; }

    public virtual Product? Product { get; set; }

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
