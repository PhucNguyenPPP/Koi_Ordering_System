using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Policy
{
    public class RefundPolicyDTO
    {
        public Guid PolicyId { get; set; }
        public string PolicyName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsBackToFarm { get; set; }
        public int PercentageRefund { get; set; }
        public bool Status { get; set; }
        public Guid FarmId { get; set; }
    }
}
