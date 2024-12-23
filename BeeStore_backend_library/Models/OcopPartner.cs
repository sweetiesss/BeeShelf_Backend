namespace BeeStore_Repository.Models;

public partial class OcopPartner : BaseEntity
{
    //public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Phone { get; set; }

    public string? CitizenIdentificationNumber { get; set; }

    public string? TaxIdentificationNumber { get; set; }

    public string? BusinessName { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? BankName { get; set; }

    public string? BankAccountNumber { get; set; }

    public string? Setting { get; set; }

    public string? PictureLink { get; set; }

    public int? RoleId { get; set; }

    public int? ProvinceId { get; set; }

    public int? CategoryId { get; set; }

    public int? OcopCategoryId { get; set; }

    public ulong? IsVerified { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<MoneyTransfer> MoneyTransfers { get; set; } = new List<MoneyTransfer>();

    public virtual OcopCategory? OcopCategory { get; set; }

    public virtual ICollection<OcopPartnerVerificationPaper> OcopPartnerVerificationPapers { get; set; } = new List<OcopPartnerVerificationPaper>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual Province? Province { get; set; }

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
}
