using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.OrderStorage
{
    public class ConfirmDeliveryDTO
    {
        public required Guid OrderId { get; set; }
        public required Guid ShipperId { get; set; }
    }
}
