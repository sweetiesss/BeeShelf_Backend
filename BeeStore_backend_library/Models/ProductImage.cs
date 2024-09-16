using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class ProductImage
{
    public int Id { get; set; }

    public string ImageLink { get; set; } = null!;

    public int? ProductId { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public ulong? IsDeleted { get; set; }

    public virtual Product? Product { get; set; }
}
