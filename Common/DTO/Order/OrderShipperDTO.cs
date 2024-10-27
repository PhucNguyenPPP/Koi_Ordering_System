using System.ComponentModel.DataAnnotations;

public class OrderShipperDTO
{
    [Key]
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = null!;
    public Guid FarmId { get; set; }
    public string FarmName { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = null!;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
}
