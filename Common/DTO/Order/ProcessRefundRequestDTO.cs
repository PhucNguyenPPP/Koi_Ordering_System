public class ProcessRefundRequestDTO
{
    public Guid OrderId { get; set; }
    public string RefundResponse { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // Accept or Deny
}
