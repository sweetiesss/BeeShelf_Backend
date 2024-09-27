using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class Inventory : BaseEntity
{
    //public int Id { get; set; }

    public string? PartnerEmail { get; set; }

    public decimal? MaxWeight { get; set; }

    public decimal? Weight { get; set; }

    public DateTime? BoughtDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public int? WarehouseId { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual ICollection<Package> Packages { get; set; } = new List<Package>();

    public virtual User? PartnerEmailNavigation { get; set; }

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual Warehouse? Warehouse { get; set; }
}
