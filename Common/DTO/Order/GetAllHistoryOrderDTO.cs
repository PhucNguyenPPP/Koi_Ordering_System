using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Order
{
    public class GetAllHistoryOrderDTO
    {
        [Key]
        public Guid OrderId { get; set; }
        public Guid FarmId { get; set; }
        public string FarmName { get; set; } = null!;
        public string OrderNumber { get; set; } = null!;
        public List<KoiOrderDTO> Kois { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = null!;
    }
}
