using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Phone { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? Picture { get; set; }

    public ulong? IsDeleted { get; set; }

    public int? RoleId { get; set; }

    public virtual Partner? Partner { get; set; }

    public virtual Role? Role { get; set; }

    public virtual WarehouseShipper? WarehouseShipper { get; set; }

    public virtual WarehouseStaff? WarehouseStaff { get; set; }
}
