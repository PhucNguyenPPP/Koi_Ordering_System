using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Dashboard
{
    public class ProfitByMonthDTO
    {
        public int Month { get; set; }
        public decimal Profit { get; set; } = 0;    
    }
}
