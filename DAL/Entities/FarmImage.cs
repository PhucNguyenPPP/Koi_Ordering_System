using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class FarmImage
{
    public Guid FarmImageId { get; set; }

    public string Link { get; set; } = null!;

    public Guid FarmId { get; set; }

    public virtual Farm Farm { get; set; } = null!;
}
