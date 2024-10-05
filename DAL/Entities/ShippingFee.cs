using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class ShippingFee
{
    public Guid ShippingFeeId { get; set; }

    public Guid StorageProvinceJpId { get; set; }

    public Guid StorageProvinceVnId { get; set; }

    public decimal Price { get; set; }

    public virtual StorageProvince StorageProvinceJp { get; set; } = null!;

    public virtual StorageProvince StorageProvinceVn { get; set; } = null!;
}
