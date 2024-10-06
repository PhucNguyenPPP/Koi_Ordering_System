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
	}
}
