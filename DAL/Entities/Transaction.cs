using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Transaction
{
    public Guid TransactionId { get; set; }

    public DateTime CreatedDate { get; set; }

    public string TransactionInfo { get; set; } = null!;

    public string TransactionNumber { get; set; } = null!;

    public string PaymentMethodTransaction { get; set; } = null!;

    public string Status { get; set; } = null!;

    public Guid OrderId { get; set; }

    public virtual Order Order { get; set; } = null!;
}
