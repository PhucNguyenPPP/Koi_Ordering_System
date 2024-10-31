using Common.DTO.General;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace Api_KoiOrderingSystem.Controllers
{
    [Route("odata")]
    [ApiController]
    public class ShippingFeeController : ControllerBase
    {
        private readonly IShippingFeeService _shippingFeeService;

        public ShippingFeeController(IShippingFeeService shippingFeeService)
        {
            _shippingFeeService = shippingFeeService;
        }

        [HttpGet("shipping-fee-province-ids")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetShippingFeeByProvinceIds(Guid storageProvinceJapanId, Guid storageProvinceVietNamId)
        {
            var result = await _shippingFeeService.GetShippingFeeByStorageProvinceId(storageProvinceJapanId, storageProvinceVietNamId);
            if(result == 0)
            {
                return NotFound(new ResponseDTO("Can not find shipping service", 404, false));
            }
            return Ok(new ResponseDTO("Get shipping fee successfully", 200, true, result));
        }
    }
}
