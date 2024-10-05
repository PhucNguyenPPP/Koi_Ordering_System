using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Breed
{
    public Guid BreedId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<KoiBreed> KoiBreeds { get; set; } = new List<KoiBreed>();
}
