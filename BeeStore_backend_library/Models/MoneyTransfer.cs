namespace BeeStore_Repository.Models;

public partial class MoneyTransfer : BaseEntity
{
    //public int Id { get; set; }

    public int? PaymentId { get; set; }

    public int? OcopPartnerId { get; set; }

    public int? TransferBy { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? CreateDate { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual OcopPartner? OcopPartner { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual Employee? TransferByNavigation { get; set; }
}
