﻿using Common.DTO.Flight;
using Common.DTO.General;
using Common.DTO.KoiFish;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Service.Interfaces;
using System;
using System.Threading.Tasks;
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
    [HttpDelete("{flightId}")]
    public async Task<IActionResult> DeleteFlight(Guid flightId)
    {
                return Created("Success", new ResponseDTO("Add flight sucessfully", 201, true, null));
            }
            else
            {
                return BadRequest(new ResponseDTO("Add flight failed", 500, false, null));
            }
        }
        bool success = await _flightService.DeleteFlight(flightId);

        [HttpPut("flight")]
        public async Task<IActionResult> UpdateFlight([FromBody] UpdateFlightDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDTO(ModelState.ToString() ?? "Unknow error", 400, false, null));
            }
            var checkValid = await _flightService.CheckValidationUpdateFlight(model);
            if (!checkValid.IsSuccess)
        if (!success)
        {
                return BadRequest(checkValid);
            // Return 400 BadRequest if the flight is assigned and cannot be deleted
            return BadRequest("Flight cannot be deleted because it is assigned to an order.");
        }

        // Return 200 OK if the flight was successfully deleted
        return Ok("Flight deleted successfully.");
            var signUpResult = await _flightService.UpdateFlight(model);
            if (signUpResult)
            {
                return Ok(new ResponseDTO("Update flight sucessfully", 201, true));
            }
            else
            {
                return BadRequest(new ResponseDTO("Update flight failed", 500, false, null));
            }
        }
    }
}
