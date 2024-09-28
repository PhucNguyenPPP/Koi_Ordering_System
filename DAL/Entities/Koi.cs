using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Koi
{
    public Guid KoiId { get; set; }

    public string Name { get; set; } = null!;

    public string CertificationLink { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime Dob { get; set; }

    public string Gender { get; set; } = null!;

    public bool Status { get; set; }

    public Guid BreedId { get; set; }

    public Guid? OrderId { get; set; }

    public Guid FarmId { get; set; }

    public virtual Breed Breed { get; set; } = null!;

    public virtual User Farm { get; set; } = null!;

    public virtual ICollection<KoiImage> KoiImages { get; set; } = new List<KoiImage>();

    public virtual Order? Order { get; set; }
}
