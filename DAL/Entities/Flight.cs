using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Flight
{
    public Guid FlightId { get; set; }

    public string Airline { get; set; } = null!;

    public DateTime DepartureDate { get; set; }

    public DateTime ArrivalDate { get; set; }

    public string Status { get; set; } = null!;

    public Guid DepartureAirportId { get; set; }

    public Guid ArrivalAirportId { get; set; }

    public virtual Airport ArrivalAirport { get; set; } = null!;

    public virtual Airport DepartureAirport { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
