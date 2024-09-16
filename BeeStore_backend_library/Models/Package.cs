using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class Package
{
    public int Id { get; set; }

    public string? PartnerEmail { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? CreateDate { get; set; }

    public float? Height { get; set; }

    public float? Length { get; set; }

    public float? Width { get; set; }

    public float? Weight { get; set; }

    public int? ProductId { get; set; }

    public int? ProductAmount { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public int? InventoryId { get; set; }

    public ulong? IsDeleted { get; set; }

    public virtual Inventory? Inventory { get; set; }

    public virtual Partner? PartnerEmailNavigation { get; set; }

    public virtual Product? Product { get; set; }

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
