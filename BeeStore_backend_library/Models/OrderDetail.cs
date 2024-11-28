namespace BeeStore_Repository.Models;

public partial class OrderDetail : BaseEntity
{
    //public int Id { get; set; }

    public int? OrderId { get; set; }

    public int? LotId { get; set; }

    public decimal? ProductPrice { get; set; }

    public int? ProductAmount { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual Lot? Lot { get; set; }

    public virtual Order? Order { get; set; }
}
