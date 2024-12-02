namespace BeeStore_Repository.Models;

public partial class Order : BaseEntity
{
    //public int Id { get; set; }

    public string? OrderCode { get; set; }

    public int? OcopPartnerId { get; set; }

    public string? Status { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? ApproveDate { get; set; }

    public DateTime? DeliverStartDate { get; set; }

    public DateTime? DeliverFinishDate { get; set; }

    public DateTime? CompleteDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public DateTime? CancelDate { get; set; }

    public string? CancellationReason { get; set; }

    public string? ReceiverPhone { get; set; }

    public string? ReceiverAddress { get; set; }

    public int? DeliveryZoneId { get; set; }

    public decimal? Distance { get; set; }

    public decimal? TotalPrice { get; set; }

    public decimal? TotalPriceAfterFee { get; set; }

    public decimal? TotalWeight { get; set; }

    public DateTime? PickDate { get; set; }

    public int? PickStaffId { get; set; }

    public string? PictureLink { get; set; }

    //public ulong? IsDeleted { get; set; }

    public int? BatchId { get; set; }

    public int? NumberOfTrips { get; set; }

    public virtual Batch? Batch { get; set; }

    public virtual DeliveryZone? DeliveryZone { get; set; }

    public virtual OcopPartner? OcopPartner { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<OrderFee> OrderFees { get; set; } = new List<OrderFee>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Employee? PickStaff { get; set; }
}
