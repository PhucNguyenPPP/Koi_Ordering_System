using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Cart
{
    public Guid CartId { get; set; }

    public Guid KoiId { get; set; }

    public Guid UserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual Koi Koi { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
