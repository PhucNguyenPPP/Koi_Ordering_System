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
		
		[HttpGet("shippers")]
        public async Task<IActionResult> GetAllShippersInStorageProvince([FromQuery] Guid storageProvinceId)
        {
            if (storageProvinceId == Guid.Empty)
            {
                return BadRequest("Invalid storage province ID.");
            }

            var shippers = await _userService.GetAllShipperInStorageProvince(storageProvinceId);
            
            if (shippers == null || shippers.Length == 0)
            {
                return NotFound("No shippers found for the specified storage province.");
            }
            
            return Ok(shippers);
        }
	}
}
