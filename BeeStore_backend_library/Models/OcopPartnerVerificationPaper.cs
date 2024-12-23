namespace BeeStore_Repository.Models;

public partial class OcopPartnerVerificationPaper : BaseEntity
{
    //public int Id { get; set; }

    public int? OcopPartnerId { get; set; }

    public DateTime? CreateDate { get; set; }

    public ulong? IsVerified { get; set; }

    public DateTime? VerifyDate { get; set; }

    public ulong? IsRejected { get; set; }

    public DateTime? RejectDate { get; set; }

    public string? RejectReason { get; set; }

    public string? FrontPictureLink { get; set; }

    public string? BackPictureLink { get; set; }

    public virtual OcopPartner? OcopPartner { get; set; }
}
