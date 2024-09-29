using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Cart
{
    public class GetCartDTO
    {
        public Guid CartId { get; set; }  
        public Guid UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string KoiName { get; set; }
        public string Price { get; set;}
    }
}
