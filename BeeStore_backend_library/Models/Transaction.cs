namespace BeeStore_Repository.Models;

public partial class Transaction : BaseEntity
{
    //public int Id { get; set; }

    public string? Code { get; set; }

    public int? Amount { get; set; }

    public string? Description { get; set; }

    public int? OcopPartnerId { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? Status { get; set; }

    public string? CancellationReason { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual OcopPartner? OcopPartner { get; set; }
}
