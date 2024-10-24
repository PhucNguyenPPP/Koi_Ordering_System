using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Order
{
    public class GetAllFarmHistoryOrderDTO
    {
        [Key]
        public Guid OrderId { get; set; }
        public Guid FarmId { get; set; }
        public string FarmName { get; set; } = null!;
        public string OrderNumber { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = null!;
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
    }
}
