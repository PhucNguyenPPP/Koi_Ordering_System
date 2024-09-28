public class PolicyDTO
{
    public Guid PolicyId { get; set; }
    public string Description { get; set; }
    public int PercentageRefund { get; set; }
    public int PercentagePrepay { get; set; }
    public int ReturnDateLimited { get; set; }
    public Guid PaymentMethodId { get; set; }
}
