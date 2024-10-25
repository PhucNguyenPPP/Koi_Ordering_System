using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Flight
{
    public class UpdateFlightDTO
    {
        [Required(ErrorMessage = "Please input FlightId")]
        public Guid FlightId { get; set; }
        [Required(ErrorMessage = "Please input FlightCode")]
        public string FlightCode { get; set; } = null!;
        
        [Required(ErrorMessage = "Please input Airline Name")]
        public string Airline { get; set; } = null!;
        [Required(ErrorMessage = "Please input DepartureDate")]
        public DateTime DepartureDate { get; set; }
        [Required(ErrorMessage = "Please input ArrivalDate")]
        public DateTime ArrivalDate { get; set; }
        [Required(ErrorMessage = "Please input DepartureAirportId")]
        public Guid DepartureAirportId { get; set; }
        [Required(ErrorMessage = "Please input ArrivalAirportId")]
        public Guid ArrivalAirportId { get; set; }
    }
}
