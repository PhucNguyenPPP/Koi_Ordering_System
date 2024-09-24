using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Farm
{
    public Guid FarmId { get; set; }

    public string FarmName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual ICollection<FarmImage> FarmImages { get; set; } = new List<FarmImage>();

    public virtual ICollection<Koi> Kois { get; set; } = new List<Koi>();
}
