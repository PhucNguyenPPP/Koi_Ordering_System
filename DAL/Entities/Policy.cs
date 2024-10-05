using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Policy
{
    public Guid PolicyId { get; set; }

    public string PolicyName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int PercentageRefund { get; set; }

    public Guid FarmId { get; set; }

    public virtual KoiFarm Farm { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
