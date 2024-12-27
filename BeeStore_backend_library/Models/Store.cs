namespace BeeStore_Repository.Models;

public partial class Store : BaseEntity
{
    //public int Id { get; set; }

    public string? Name { get; set; }

    public decimal? Capacity { get; set; }

    public string? Location { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }
    public int? ProvinceId { get; set; }

    public DateTime? CreateDate { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual Province? Province { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    public virtual ICollection<StoreShipper> StoreShippers { get; set; } = new List<StoreShipper>();

    public virtual ICollection<StoreStaff> StoreStaffs { get; set; } = new List<StoreStaff>();

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
