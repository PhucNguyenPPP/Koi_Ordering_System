using Common.DTO.General;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace Api_KoiOrderingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BreedController : ControllerBase
    {
        private readonly IBreedService _breedService;
        public BreedController(IBreedService breedService)
        {
            _breedService = breedService;
        }

        [HttpGet("all-breeds")]
        [Authorize(Roles = "KoiFarmManager,Admin")]
        public IActionResult GetAllBreed()
        {
            var list = _breedService.GetAllBreeds();
            if (list.Count > 0)
            {
                return Ok(new ResponseDTO("Get all breeds successfully", 200, true, list));
            }
            return NotFound(new ResponseDTO("Breed list is null", 404, false));
        }

        [HttpPost("new-breed")]
        public async Task<IActionResult> CreateBreed(string breedName)
        {
            var result = await _breedService.AddBreed(breedName);
            if (result)
            {
                return Created("Create breed successfully", new ResponseDTO("Creat breed successfully", 201, true));
            }
            return BadRequest(new ResponseDTO("Creat breed unsuccessfully", 400, false));
        }
    }
}
