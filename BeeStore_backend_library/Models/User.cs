using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class User : BaseEntity
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Phone { get; set; }

    public DateTime? CreateDate { get; set; }

    public int? PictureId { get; set; }

    public string? Setting { get; set; }

    public int? RoleId { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Partner> Partners { get; set; } = new List<Partner>();

    public virtual Picture? Picture { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual ICollection<WarehouseShipper> WarehouseShippers { get; set; } = new List<WarehouseShipper>();

    public virtual ICollection<WarehouseStaff> WarehouseStaffs { get; set; } = new List<WarehouseStaff>();
}
