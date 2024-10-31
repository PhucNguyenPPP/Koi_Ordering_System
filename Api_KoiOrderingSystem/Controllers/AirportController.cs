using Common.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Service.Interfaces;

namespace Api_KoiOrderingSystem.Controllers
{
    [Route("odata")]
    [ApiController]
    public class AirportController : ODataController
    {
        private readonly IAirportService _airportService;

        public AirportController(IAirportService airportService)
        {
            _airportService = airportService;
        }

        [HttpGet("all-airports")]
        [EnableQuery]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllAirports()
        {
            var list = _airportService.GetAllAirports();
            return Ok(list);
        }
    }
}
