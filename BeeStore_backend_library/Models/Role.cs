using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class Role : BaseEntity
{
    //public int Id { get; set; }

    public string? RoleName { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
