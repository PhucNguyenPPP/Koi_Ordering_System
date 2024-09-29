using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Cart
{
    public class CartDTO
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid KoiId { get; set; }
    }
}
