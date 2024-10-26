namespace BeeStore_Repository.Models;

public partial class Picture : BaseEntity //DELETE THIS
{
    //public int Id { get; set; }

    public string? PictureLink { get; set; }

    public DateTime? CreateDate { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
