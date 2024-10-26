namespace BeeStore_Repository.Models;

public partial class Partner : BaseEntity //DELETE THIS
{
    //public int Id { get; set; }

    public string? CitizenIdentificationNumber { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? BankName { get; set; }

    public string? BankAccountNumber { get; set; }

    public int? UserId { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual User? User { get; set; }
}
