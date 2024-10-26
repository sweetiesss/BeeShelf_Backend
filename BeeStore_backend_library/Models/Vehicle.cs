using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class Vehicle : BaseEntity
{
    //public int Id { get; set; }

    public string? Name { get; set; }

    public string? LicensePlate { get; set; }

    public int? Capacity { get; set; }

    public string? Type { get; set; }

    public string? Status { get; set; }

    public int? AssignedDriverId { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual Employee? AssignedDriver { get; set; }
}
