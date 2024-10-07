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
		

	}
}
