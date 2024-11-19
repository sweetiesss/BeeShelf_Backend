namespace BeeStore_Repository.Models;

public partial class Product : BaseEntity
{
    //public int Id { get; set; }

    public int? OcopPartnerId { get; set; }

    public string? Barcode { get; set; }

    public string? Name { get; set; }

    public decimal? Price { get; set; }

    public decimal? Weight { get; set; }

    public DateTime? CreateDate { get; set; }

    public int? ProductCategoryId { get; set; }

    public string? PictureLink { get; set; }

    public string? Origin { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual ICollection<Lot> Lots { get; set; } = new List<Lot>();

    public virtual OcopPartner? OcopPartner { get; set; }

    public virtual ProductCategory? ProductCategory { get; set; }
}
