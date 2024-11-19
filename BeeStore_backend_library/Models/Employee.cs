using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class Employee : BaseEntity
{
    //public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Phone { get; set; }

    public string? Status { get; set; }

    public DateTime? CreateDate { get; set; }

    //public ulong? IsDeleted { get; set; }

    public string? Setting { get; set; }

    public string? PictureLink { get; set; }

    public int? RoleId { get; set; }

    public virtual ICollection<BatchDelivery> BatchDeliveries { get; set; } = new List<BatchDelivery>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

    public virtual ICollection<WarehouseShipper> WarehouseShippers { get; set; } = new List<WarehouseShipper>();

    public virtual ICollection<WarehouseStaff> WarehouseStaffs { get; set; } = new List<WarehouseStaff>();
}
