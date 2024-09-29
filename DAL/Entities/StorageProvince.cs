using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class StorageProvince
{
    public Guid StorageProvinceId { get; set; }

    public string StorageName { get; set; } = null!;

    public string ProvinceName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Country { get; set; } = null!;

    public bool Status { get; set; }

    public Guid AirportId { get; set; }

    public virtual Airport Airport { get; set; } = null!;

    public virtual ICollection<OrderStorage> OrderStorages { get; set; } = new List<OrderStorage>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
