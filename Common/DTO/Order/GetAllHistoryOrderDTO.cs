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
        public string OrderNumber { get; set; } = null!;
        public List<string> AvatarLink { get; set; } = null!;
        public List<string> KoiName { get; set; } = null!;
        public string FarmName { get; set; } = null!;
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = null!;
    }
}
