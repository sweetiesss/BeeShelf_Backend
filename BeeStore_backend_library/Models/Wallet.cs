namespace BeeStore_Repository.Models;

public partial class Wallet : BaseEntity
{
    //public int Id { get; set; }

    public int? OcopPartnerId { get; set; }

    public decimal? TotalAmount { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual OcopPartner? OcopPartner { get; set; }
}
