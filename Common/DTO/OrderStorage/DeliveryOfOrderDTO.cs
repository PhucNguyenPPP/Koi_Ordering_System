using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.OrderStorage
{
    public class DeliveryOfOrderDTO
    {
        public required string Status { get; set; }
        public DateTime? ArrivalTime { get; set; }
    }
}
