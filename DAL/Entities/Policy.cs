using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Policy
{
    public Guid PolicyId { get; set; }

    public string Description { get; set; } = null!;

    public int PercentageRefund { get; set; }

    public int PercentagePrepay { get; set; }

    public int ReturnDateLimited { get; set; }

    public Guid PaymentMethodId { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;
}
