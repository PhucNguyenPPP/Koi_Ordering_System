using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class RefundRequestMedium
{
    public Guid RefundRequestMediaId { get; set; }

    public string Link { get; set; } = null!;

    public Guid OrderId { get; set; }

    public virtual Order Order { get; set; } = null!;
}
