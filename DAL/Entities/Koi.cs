using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Koi
{
    public Guid KoiId { get; set; }

    public string Name { get; set; } = null!;

    public string AvatarLink { get; set; } = null!;

    public string CertificationLink { get; set; } = null!;

    public decimal Price { get; set; }

    public string Description { get; set; } = null!;

    public DateTime Dob { get; set; }

    public string Gender { get; set; } = null!;

    public bool Status { get; set; }

    public Guid FarmId { get; set; }

    public Guid? OrderId { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual KoiFarm Farm { get; set; } = null!;

    public virtual ICollection<KoiBreed> KoiBreeds { get; set; } = new List<KoiBreed>();

    public virtual ICollection<KoiImage> KoiImages { get; set; } = new List<KoiImage>();

    public virtual Order? Order { get; set; }
}
