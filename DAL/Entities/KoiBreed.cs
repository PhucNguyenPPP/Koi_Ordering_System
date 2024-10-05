using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class KoiBreed
{
    public Guid KoiBreedId { get; set; }

    public Guid BreedId { get; set; }

    public Guid KoiId { get; set; }

    public virtual Breed Breed { get; set; } = null!;

    public virtual Koi Koi { get; set; } = null!;
}
