using System.ComponentModel.DataAnnotations;

public class PolicyDTO
{
    [Key]
    public Guid PolicyId { get; set; }
    public string PolicyName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool IsBackToFarm { get; set; }

    public int PercentageRefund { get; set; }
    public bool Status { get; set; }
    public Guid FarmId { get; set; }
}
