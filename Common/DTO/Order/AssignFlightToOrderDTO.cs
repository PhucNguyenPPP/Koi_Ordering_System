using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Order
{
    public class AssignFlightToOrderDTO
    {
        [Required(ErrorMessage = "Please input FlightId")]
        public Guid FlightId { get; set; }
        [Required(ErrorMessage = "Please input OrderId")]
        public Guid OrderId { get; set; }
    }
}
