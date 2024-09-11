using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class Partner
{
    public int Id { get; set; }

    public string CitizenIdentificationNumber { get; set; } = null!;

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? BankName { get; set; }

    public string? BankAccountNumber { get; set; }

    public string UserEmail { get; set; } = null!;

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User UserEmailNavigation { get; set; } = null!;
}
