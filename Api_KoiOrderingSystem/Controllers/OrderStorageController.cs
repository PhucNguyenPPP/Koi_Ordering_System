using Common.DTO.OrderStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace Api_KoiOrderingSystem.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class OrderStorageController : ControllerBase
    {
        private readonly IOrderStorageService _orderStorageService;
        public OrderStorageController(IOrderStorageService orderStorageService)
        {
            _orderStorageService = orderStorageService;
        }
        [HttpPut]
        public async Task<IActionResult> AssignShipper([FromBody] AssignShipperDTO assignShipperDTO)
        {
            var result = await _orderStorageService.AssignShipper(assignShipperDTO);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        [HttpPut("delivery")]
        public async Task<IActionResult> ConfirmDelivery([FromBody] ConfirmDeliveryDTO confirmDeliveryDTO)
        {
            var result = await _orderStorageService.ConfirmDelivery(confirmDeliveryDTO);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
