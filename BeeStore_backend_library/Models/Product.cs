using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? Amount { get; set; }

    public float? Weight { get; set; }

    public DateTime? CreateDate { get; set; }

    public int? CategoryId { get; set; }

    public ulong? IsDeleted { get; set; }

    public virtual CategoryType? Category { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Package> Packages { get; set; } = new List<Package>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
}
