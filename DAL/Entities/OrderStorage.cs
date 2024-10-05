using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class OrderStorage
{
    public Guid OrderStorageId { get; set; }

    public DateTime? ArrivalTime { get; set; }

    public bool Status { get; set; }

    public Guid StorageProvinceId { get; set; }

    public Guid OrderId { get; set; }

    public Guid? ShipperId { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual User? Shipper { get; set; }

    public virtual StorageProvince StorageProvince { get; set; } = null!;
}
