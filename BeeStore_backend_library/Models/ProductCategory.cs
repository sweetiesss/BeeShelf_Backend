namespace BeeStore_Repository.Models;

public partial class ProductCategory : BaseEntity
{
    //public int Id { get; set; }

    public int? CategoryId { get; set; }

    public string? TypeName { get; set; }

    public string? TypeDescription { get; set; }

    public int? ExpireIn { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
