using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using System;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class FlightController : ControllerBase
{
    private readonly IFlightService _flightService;

    public FlightController(IFlightService flightService)
    {
        _flightService = flightService;
    }

    [HttpDelete("{flightId}")]
    public async Task<IActionResult> DeleteFlight(Guid flightId)
    {
        bool success = await _flightService.DeleteFlight(flightId);

        if (!success)
        {
            // Return 400 BadRequest if the flight is assigned and cannot be deleted
            return BadRequest("Flight cannot be deleted because it is assigned to an order.");
        }

        // Return 200 OK if the flight was successfully deleted
        return Ok("Flight deleted successfully.");
    }
}
