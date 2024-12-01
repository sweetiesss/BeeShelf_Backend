namespace BeeStore_Repository.Models;

public partial class Payment : BaseEntity
{
    //public int Id { get; set; }

    public int? OcopPartnerId { get; set; }

    public int? OrderId { get; set; }

    public int? CollectedBy { get; set; }

    public decimal? TotalAmount { get; set; }

    public ulong? IsConfirmed { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual Employee? CollectedByNavigation { get; set; }

    public virtual OcopPartner? OcopPartner { get; set; }

    public virtual Order? Order { get; set; }
}
