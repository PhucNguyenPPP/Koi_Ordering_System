using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class RefundRequest
{
    public Guid RefundRequestId { get; set; }

    public string Description { get; set; } = null!;

    public string Response { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string Status { get; set; } = null!;

    public Guid OrderId { get; set; }

    public Guid PolicyId { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Policy Policy { get; set; } = null!;

    public virtual ICollection<RefundRequestMedium> RefundRequestMedia { get; set; } = new List<RefundRequestMedium>();
}
