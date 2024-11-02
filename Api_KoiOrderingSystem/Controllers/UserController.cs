using Common.DTO.General;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Service.Interfaces;
using Service.Services;

namespace Api_KoiOrderingSystem.Controllers
{
	[Route("odata")]
	[ApiController]
	public class UserController : ODataController
	{
		private readonly IUserService _userService;
		public UserController(IUserService userService)
		{
			_userService = userService;
		}
		
		[HttpGet("shippers")]
        [Authorize(Roles = "StorageManager,Admin")]
        [EnableQuery]
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
