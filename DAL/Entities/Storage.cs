using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Storage
{
    public Guid StorageId { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public bool Status { get; set; }

    public virtual ICollection<OrderStorage> OrderStorages { get; set; } = new List<OrderStorage>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
