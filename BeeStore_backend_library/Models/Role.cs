using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class Role : BaseEntity
{
    //public int Id { get; set; }

    public string? RoleName { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<OcopPartner> OcopPartners { get; set; } = new List<OcopPartner>();
}
