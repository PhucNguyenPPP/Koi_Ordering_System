public class PolicyDTO
{
    public Guid PolicyId { get; set; }
    public string PolicyName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int PercentageRefund { get; set; }
    public Guid FarmId { get; set; }
}
