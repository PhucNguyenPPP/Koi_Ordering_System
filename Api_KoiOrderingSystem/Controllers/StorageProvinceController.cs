using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Api_KoiOrderingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageProvinceController : ControllerBase
    {

        private readonly IStorageProvinceService _storageProvinceService;
        public StorageProvinceController(IStorageProvinceService storageProvinceService)
        {
            _storageProvinceService = storageProvinceService;
        }

        [HttpGet]
        public IActionResult GetAllStorageProvinceByCountry(string? country)
        {
            var result = _storageProvinceService.GetStorageProvinceByContry(country);
            if(result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
