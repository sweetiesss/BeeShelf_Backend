using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class Image
{
    public int Id { get; set; }

    public string? ImageLink { get; set; }

    public int ProductId { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }
}
