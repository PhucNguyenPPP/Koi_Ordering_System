using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Airport
{
    public Guid AirportId { get; set; }

    public string AirportName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Country { get; set; } = null!;

    public virtual ICollection<Flight> FlightArrivalAirports { get; set; } = new List<Flight>();

    public virtual ICollection<Flight> FlightDepartureAirports { get; set; } = new List<Flight>();

    public virtual ICollection<StorageProvince> StorageProvinces { get; set; } = new List<StorageProvince>();
}
