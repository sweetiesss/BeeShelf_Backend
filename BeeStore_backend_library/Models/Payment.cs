using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class Payment : BaseEntity
{
    //public int Id { get; set; }

    public int? WalletId { get; set; }

    public int? OrderId { get; set; }

    public int? TotalAmount { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Wallet? Wallet { get; set; }
}
