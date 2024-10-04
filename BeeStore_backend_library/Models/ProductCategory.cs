using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class ProductCategory : BaseEntity
{

    public string? TypeName { get; set; }

    public string? TypeDescription { get; set; }

    public int? ExpireIn { get; set; }


    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<WarehouseCategory> WarehouseCategories { get; set; } = new List<WarehouseCategory>();
}
