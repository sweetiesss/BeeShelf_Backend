using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class WarehouseCategory : BaseEntity
{

    public int? WarehouseId { get; set; }

    public int? ProductCategoryId { get; set; }


    public virtual ProductCategory? ProductCategory { get; set; }

    public virtual Warehouse? Warehouse { get; set; }
}
