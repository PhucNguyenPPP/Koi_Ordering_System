using Common.DTO.General;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Services;

namespace Api_KoiOrderingSystem.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;
		public UserController(IUserService userService)
		{
			_userService = userService;
		}
		[HttpGet("farm")]
		public async Task<IActionResult> GetFarmDetail(Guid userId)
		{
			ResponseDTO responseDTO = await _userService.GetFarmDetail(userId);
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
