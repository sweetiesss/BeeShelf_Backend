namespace BeeStore_Repository.Models;

public partial class Province : BaseEntity
{
    //public int Id { get; set; }

    public string? Code { get; set; }

    public string? SubDivisionName { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual ICollection<DeliveryZone> DeliveryZones { get; set; } = new List<DeliveryZone>();

    public virtual ICollection<OcopPartner> OcopPartners { get; set; } = new List<OcopPartner>();

    public virtual ICollection<Store> Stores { get; set; } = new List<Store>();
}
