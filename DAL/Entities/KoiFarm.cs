using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class KoiFarm
{
    public Guid KoiFarmId { get; set; }

    public string FarmName { get; set; } = null!;

    public string FarmDescription { get; set; } = null!;

    public string FarmAddress { get; set; } = null!;

    public string FarmAvatar { get; set; } = null!;

    public Guid? StorageProvinceId { get; set; }

    public Guid KoiFarmManagerId { get; set; }

    public virtual User KoiFarmManager { get; set; } = null!;

    public virtual ICollection<Koi> Kois { get; set; } = new List<Koi>();

    public virtual ICollection<Policy> Policies { get; set; } = new List<Policy>();

    public virtual StorageProvince? StorageProvince { get; set; }
}
