namespace BeeStore_Repository.Models;

public partial class ExportFee : BaseEntity
{
    //public int Id { get; set; }

    public int? RequestId { get; set; }

    public decimal? DeliveryFee { get; set; }

    public decimal? AdditionalFee { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual Request? Request { get; set; }
}
