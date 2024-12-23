namespace BeeStore_Repository.Models;

public partial class Room : BaseEntity
{
    //public int Id { get; set; }

    public int? OcopPartnerId { get; set; }

    public string? RoomCode { get; set; }

    public decimal? MaxWeight { get; set; }

    public decimal? Weight { get; set; }

    public decimal? Price { get; set; }

    public DateTime? BoughtDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public int? StoreId { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual ICollection<Lot> Lots { get; set; } = new List<Lot>();

    public virtual OcopPartner? OcopPartner { get; set; }

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual Store? Store { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
