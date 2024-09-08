using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class Category
{
    public int Id { get; set; }

    public string TypeName { get; set; } = null!;

    public int? DateToExpire { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
