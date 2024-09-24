using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class KoiImage
{
    public Guid KoiImageId { get; set; }

    public string Link { get; set; } = null!;

    public Guid KoiId { get; set; }

    public virtual Koi Koi { get; set; } = null!;
}
