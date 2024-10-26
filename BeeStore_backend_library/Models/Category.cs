using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class Category : BaseEntity
{
    //public int Id { get; set; }

    public string? Type { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual ICollection<OcopPartner> OcopPartners { get; set; } = new List<OcopPartner>();

    public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
}
