using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class Role : BaseEntity
{
    public string? RoleName { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
