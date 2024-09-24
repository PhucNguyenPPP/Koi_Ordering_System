using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class OrderStorage
{
    public Guid OrderStorageId { get; set; }

    public DateTime ArrivalTime { get; set; }

    public bool Status { get; set; }

    public Guid StorageId { get; set; }

    public Guid OrderId { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Storage Storage { get; set; } = null!;
}
