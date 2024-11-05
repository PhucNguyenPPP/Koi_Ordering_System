using Common.DTO.General;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Api_KoiOrderingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        [HttpGet("revenue-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRevenueByAdmin([Required] DateOnly startdate,
                                                           [Required] DateOnly enddate)
        {
            ResponseDTO responseDTO = await _dashboardService.GetRevenueByAdmin(startdate, enddate);
            if (responseDTO.IsSuccess == false)
            {
                if (responseDTO.StatusCode == 404)
                {
                    return NotFound(responseDTO);
                }
                else
                {
                    return BadRequest(responseDTO);
                }
            }

            return Ok(responseDTO);
        }

        [HttpGet("revenue-farm")]
        [Authorize(Roles = "KoiFarmManager")]
        public async Task<IActionResult> GetRevenueByFarm([Required] DateOnly startdate,
                                                          [Required] DateOnly enddate,
                                                          [Required] Guid farmId)
        {
            ResponseDTO responseDTO = await _dashboardService.GetRevenueByFarm(startdate, enddate, farmId);
            if (responseDTO.IsSuccess == false)
            {
                if (responseDTO.StatusCode == 404)
                {
                    return NotFound(responseDTO);
                }
                else
                {
                    return BadRequest(responseDTO);
                }
            }

            return Ok(responseDTO);
        }
        [HttpGet("profit")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetProfitByAdmin([Required] DateOnly startdate,
                                                          [Required] DateOnly enddate)
        {
            ResponseDTO responseDTO = await _dashboardService.GetProfitByAdmin(startdate, enddate);
            if (responseDTO.IsSuccess == false)
            {
                if (responseDTO.StatusCode == 404)
                {
                    return NotFound(responseDTO);
                }
                else
                {
                    return BadRequest(responseDTO);
                }
            }

            return Ok(responseDTO);
        }
        [Authorize(Roles ="Admin")]
        [HttpGet("profit-of-admin")]
        public async Task<IActionResult> GetProfitOfAdminByYear([Required] int year)
        {
            ResponseDTO responseDTO = await _dashboardService.GetProfitOfAdminByYear(year);
            if (responseDTO.IsSuccess == false)
            {
                if (responseDTO.StatusCode == 404)
                {
                    return NotFound(responseDTO);
                }
                else
                {
                    return BadRequest(responseDTO);
                }
            }

            return Ok(responseDTO);
        }
        [Authorize(Roles = "KoiFarmManager")]
        [HttpGet("profit-of-farm")]
        public async Task<IActionResult> GetProfitOfFarmByYear([Required] int year, [Required] Guid farmId)
        {
            ResponseDTO responseDTO = await _dashboardService.GetProfitOfFarmByYear(year,farmId);
            if (responseDTO.IsSuccess == false)
            {
                if (responseDTO.StatusCode == 404)
                {
                    return NotFound(responseDTO);
                }
                else
                {
                    return BadRequest(responseDTO);
                }
            }

            return Ok(responseDTO);
        }
        [Authorize(Roles = "KoiFarmManager")]
        [HttpGet("profit-of-farm-by-time-range")]
        public async Task<IActionResult> GetProfitOfFarmByTimeRange([Required] DateOnly startdate,
                                                          [Required] DateOnly enddate, [Required] Guid farmId)
        {
            ResponseDTO responseDTO = await _dashboardService.GetProfitOfFarmByTimeRange(startdate,enddate, farmId);
            if (responseDTO.IsSuccess == false)
            {
                if (responseDTO.StatusCode == 404)
                {
                    return NotFound(responseDTO);
                }
                else
                {
                    return BadRequest(responseDTO);
                }
            }

            return Ok(responseDTO);
        }
    }
}
