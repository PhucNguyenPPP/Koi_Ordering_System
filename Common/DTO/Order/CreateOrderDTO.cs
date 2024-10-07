using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Order
{
    public class CreateOrderDTO
    {
        [Required(ErrorMessage = "Please input phone number")]
        [RegularExpression("^0\\d{9}$", ErrorMessage = "Phone number is invalid")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Please input address")]
        public string Address { get; set; } = null!;

        public int? Rating { get; set; }

        public string? Feedback { get; set; }

        [Required(ErrorMessage = "Please input customer ID")]
        public int? Weight { get; set; }

        [Required(ErrorMessage = "Please input customer ID")]
        public int? Width { get; set; }

        [Required(ErrorMessage = "Please input customer ID")]
        public int? Height { get; set; }

        [Required(ErrorMessage = "Please input customer ID")]
        public int? Length { get; set; }

        [Required(ErrorMessage = "Please input customer ID")]
        public Guid CustomerId { get; set; }

        [Required(ErrorMessage = "Please input policy ID")]
        public Guid PolicyId { get; set; }

        [Required(ErrorMessage = "Please input koi ID")]
        public List<Guid> KoiId { get; set; }
    }
}
