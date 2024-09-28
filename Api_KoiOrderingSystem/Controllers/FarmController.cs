using Common.DTO.General;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Services;

namespace Api_KoiOrderingSystem.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FarmController : ControllerBase
	{
		private readonly IFarmService _farmService;
		public FarmController(IFarmService farmService)
		{
			_farmService = farmService;
		}

	}
}
