using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class Partner : BaseEntity
{
    //public int Id { get; set; }

    public string? CitizenIdentificationNumber { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? BankName { get; set; }

    public string? BankAccountNumber { get; set; }

    public string? UserEmail { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual User? UserEmailNavigation { get; set; }
}
