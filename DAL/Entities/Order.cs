using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Order
{
    public Guid OrderId { get; set; }

    public string OrderNumber { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string Phone { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string ShippingFee { get; set; } = null!;

    public decimal TotalPrice { get; set; }

    public int? Rating { get; set; }

    public string? Feedback { get; set; }

    public int? Weight { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public int? Length { get; set; }

    public string Status { get; set; } = null!;

    public Guid CustomerId { get; set; }

    public Guid PolicyId { get; set; }

    public Guid? FlightId { get; set; }

    public virtual User Customer { get; set; } = null!;

    public virtual Flight? Flight { get; set; }

    public virtual ICollection<Koi> Kois { get; set; } = new List<Koi>();

    public virtual ICollection<OrderStorage> OrderStorages { get; set; } = new List<OrderStorage>();

    public virtual Policy Policy { get; set; } = null!;

    public virtual ICollection<RefundRequest> RefundRequests { get; set; } = new List<RefundRequest>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
