﻿using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class Request : BaseEntity
{
    //public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public int? PackageId { get; set; }

    public int? SendToInventory { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? Status { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual Package? Package { get; set; }

    public virtual Inventory? SendToInventoryNavigation { get; set; }
}
