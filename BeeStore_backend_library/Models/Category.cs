﻿namespace BeeStore_Repository.Models;

public partial class Category : BaseEntity
{
    //public int Id { get; set; }

    public string? Type { get; set; }

    public int? OcopCategoryId { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual OcopCategory? OcopCategory { get; set; }

    public virtual ICollection<OcopPartner> OcopPartners { get; set; } = new List<OcopPartner>();

    public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
}
