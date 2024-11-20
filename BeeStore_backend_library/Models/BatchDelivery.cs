namespace BeeStore_Repository.Models;

public partial class BatchDelivery : BaseEntity
{
    //public int Id { get; set; }

    public int? NumberOfTrips { get; set; }

    public DateTime? DeliveryStartDate { get; set; }

    public int? BatchId { get; set; }

    public int? DeliverBy { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual Batch? Batch { get; set; }

    public virtual Employee? DeliverByNavigation { get; set; }
}
