using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class Product : BaseEntity
{

    public int? UserId { get; set; }

    public string? Name { get; set; }

    public decimal? Price { get; set; }

    public decimal? Weight { get; set; }

    public DateTime? CreateDate { get; set; }

    public int? ProductAmount { get; set; }

    public int? PictureId { get; set; }

    public int? ProductCategoryId { get; set; }

    public string? Origin { get; set; }


    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Package> Packages { get; set; } = new List<Package>();

    public virtual Picture? Picture { get; set; }

    public virtual ProductCategory? ProductCategory { get; set; }

    public virtual User? User { get; set; }
}
