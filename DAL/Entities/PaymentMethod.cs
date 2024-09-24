using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class PaymentMethod
{
    public Guid PaymentMethodId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Policy> Policies { get; set; } = new List<Policy>();
}
