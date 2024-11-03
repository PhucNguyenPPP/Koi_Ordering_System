using Microsoft.AspNetCore.Http;

public class CreateRefundRequestDTO
{
    public Guid OrderId { get; set; }
    public string RefundDescription { get; set; } = "";
    public string BankAccount { get; set; } = "";
    public Guid PolicyId { get; set; }
    public List<IFormFile> Images { get; set; } = null!;
}