using Common.DTO.General;
using DAL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_KoiOrderingSystem.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class KoiFarmController : ControllerBase
	{
		private readonly IKoiFarmService _koiFarmService;
		public KoiFarmController(IKoiFarmService koiFarmService)
		{
			_koiFarmService = koiFarmService;
		}
		[HttpGet("farm")]
		public async Task<IActionResult> GetFarmDetail(Guid farmId)
		{
			ResponseDTO responseDTO = await _koiFarmService.GetFarmDetail(farmId);
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
	}
}
