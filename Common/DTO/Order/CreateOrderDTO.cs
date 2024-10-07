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

        [Required(ErrorMessage = "Please input customer ID")]
        public Guid CustomerId { get; set; }

        [Required(ErrorMessage = "Please input Viet Nam storage province ID")]
        public Guid StorageVietNamId { get; set; }

        [Required(ErrorMessage = "Please input cart ID")]
        public List<Guid> CartId { get; set; }
    }
}
