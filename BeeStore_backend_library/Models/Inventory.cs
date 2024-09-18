using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class Inventory
{
    public int Id { get; set; }

    public string? PartnerEmail { get; set; }

    public float? Height { get; set; }

    public float? Width { get; set; }

    public float? Length { get; set; }

    public float? MaxWeight { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public int? WarehouseId { get; set; }

    public ulong? IsDeleted { get; set; }

    public virtual ICollection<Package> Packages { get; set; } = new List<Package>();

    public virtual Partner? PartnerEmailNavigation { get; set; }

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual Warehouse? Warehouse { get; set; }
}
