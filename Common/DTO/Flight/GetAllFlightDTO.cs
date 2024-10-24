using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Flight
{
    public class GetAllFlightDTO
    {
        [Key]
        public Guid FlightId { get; set; }

        public string Airline { get; set; } = null!;

        public DateTime DepartureDate { get; set; }

        public DateTime ArrivalDate { get; set; }

        public string Status { get; set; } = null!;

        public Guid DepartureAirportId { get; set; }

        public Guid ArrivalAirportId { get; set; }
        public string DepartureAirportName { get; set; } = null!;

        public string ArrivalAirportName { get; set; } = null!;
    }
}

