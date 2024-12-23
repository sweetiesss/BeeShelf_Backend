namespace BeeStore_Repository.Models;

public partial class MoneyTransfer : BaseEntity
{
    //public int Id { get; set; }

    public int? OcopPartnerId { get; set; }

    public int? TransferBy { get; set; }

    public DateTime? ConfirmDate { get; set; }

    public string? CancellationReason { get; set; }

    public ulong? IsTransferred { get; set; }

    public decimal? Amount { get; set; }

    public string? PictureLink { get; set; }

    public DateTime? CreateDate { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual OcopPartner? OcopPartner { get; set; }

    public virtual Employee? TransferByNavigation { get; set; }
}
