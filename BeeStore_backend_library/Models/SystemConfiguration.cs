namespace BeeStore_Repository.Models;

public partial class SystemConfiguration : BaseEntity
{
    //public int Id { get; set; }

    public decimal? DeliveryWeigtTreshold { get; set; }

    public decimal? DeliveryWeightUnit { get; set; }

    public decimal? DeliveryWeightFeeRate { get; set; }

    public decimal? DeliveryDistanceTreshold { get; set; }

    public decimal? DeliveryDistanceUnit { get; set; }

    public decimal? DeliveryDistanceFeeRate { get; set; }

    public decimal? StorageFeeFirstPeriodRate { get; set; }

    public decimal? StorageFeeFirstPeriodDuration { get; set; }

    public decimal? StorageFeeSecondPeriodRate { get; set; }

    public decimal? StorageFeeSecondPeriodDuration { get; set; }

    public decimal? StorageFeePersonalFeePerLot { get; set; }

    public decimal? AdditonalUpdateFeeRatePercentage { get; set; }

    public decimal? ExportDeliveryDistanceTreshold { get; set; }

    public decimal? ExportDeliveryDistanceFeeRate { get; set; }

    public decimal? ExportDeliveryDistanceUnit { get; set; }

    public decimal? ExportDeliveryWeightTreshold { get; set; }

    public decimal? ExportDeliveryWeightFeeRate { get; set; }

    public decimal? ExportDeliveryWeightUnit { get; set; }

    public decimal? OrderDistanceLimit { get; set; }

    public decimal? RequestDistanceLimit { get; set; }

    public decimal? OrderWeightLimit { get; set; }

    public decimal? RequestWeightLimit { get; set; }

    public int? InventoryExpireEmailNotificationPeriod { get; set; }

    //public ulong? IsDeleted { get; set; }
}
