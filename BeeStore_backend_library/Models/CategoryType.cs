using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class CategoryType
{
    public int Id { get; set; }

    public string TypeName { get; set; } = null!;

    public string? TypeDescription { get; set; }

    public int? ExpireIn { get; set; }

    public ulong? IsDeleted { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
