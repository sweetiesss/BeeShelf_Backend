namespace BeeStore_Repository.Models;

public partial class OcopCategory : BaseEntity
{
    //public int Id { get; set; } 

    public string? Type { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<OcopPartner> OcopPartners { get; set; } = new List<OcopPartner>();
}
