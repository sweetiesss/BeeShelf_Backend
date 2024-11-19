using System;
using System.Collections.Generic;

namespace BeeStore_Repository.Models;

public partial class Province : BaseEntity
{
    //public int Id { get; set; }

    public string? Code { get; set; }

    public string? SubDivisionName { get; set; }

    public string? SubDivsisionCategory { get; set; }

    //public ulong? IsDeleted { get; set; }

    public virtual ICollection<OcopPartner> OcopPartners { get; set; } = new List<OcopPartner>();
}
