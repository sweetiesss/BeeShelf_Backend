using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class Inventory
{
    public int Id { get; set; }

    public string PartnerEmail { get; set; } = null!;

    public int? AmountOfItem { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public int WarehouseId { get; set; }

    public virtual Partner PartnerEmailNavigation { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual Warehouse Warehouse { get; set; } = null!;
}
