using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class Product
{
    public int Id { get; set; }

    public string? Barcode { get; set; }

    public string Name { get; set; } = null!;

    public int? Amount { get; set; }

    public float? Weight { get; set; }

    public float? Volume { get; set; }

    public DateTime? CreateDate { get; set; }

    public bool? Status { get; set; }

    public int? CategoryId { get; set; }

    public int? InventoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Inventory? Inventory { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
