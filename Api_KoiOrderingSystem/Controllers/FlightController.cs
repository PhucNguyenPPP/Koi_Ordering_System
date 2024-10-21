using Common.DTO.Flight;
using Common.DTO.General;
using Common.DTO.KoiFish;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Service.Interfaces;
using Service.Services;

namespace Api_KoiOrderingSystem.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly IFlightService _flightService;
        public FlightController(IFlightService flightService)
        {
            _flightService = flightService;
        }

        [HttpGet("flights")]
        [EnableQuery]
        public async Task<IActionResult> GetAllFlight()
        {
            ResponseDTO responseDTO = await _flightService.GetAll();
            if (responseDTO.IsSuccess == false)
            {
                if (responseDTO.StatusCode == 404)
                {
                    return NotFound(responseDTO);
                }
                return BadRequest(responseDTO);

            }
            return Ok(responseDTO);
        }
        [HttpPost("new-flight")]
        public async Task<IActionResult> AddFlight([FromBody] NewFlightDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDTO(ModelState.ToString() ?? "Unknow error", 400, false, null));
            }

            var checkValid = await _flightService.CheckValidationCreateFlight(model);
            if (!checkValid.IsSuccess)
            {
                return BadRequest(checkValid);
            }

            var signUpResult = await _flightService.AddFlight(model);
            if (signUpResult)
            {
                return Created("Success", new ResponseDTO("Add flight sucessfully", 201, true, null));
            }
            else
            {
                return BadRequest(new ResponseDTO("Add flight failed", 500, false, null));
            }
        }
    }
}
