using Common.DTO.OrderStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Service.Interfaces;

namespace Api_KoiOrderingSystem.Controllers
{
    [Route("odata")]
    [ApiController]
    public class OrderStorageController : ODataController
    {
        private readonly IOrderStorageService _orderStorageService;
        public OrderStorageController(IOrderStorageService orderStorageService)
        {
            _orderStorageService = orderStorageService;
        }
        [HttpPut("Japanese-shipper")]
        public async Task<IActionResult> AssignShipperJapan([FromBody] AssignShipperDTO assignShipperDTO)
        {
            var result = await _orderStorageService.AssignShipperJapan(assignShipperDTO);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        [HttpPut("Vietnamese-shipper")]
        public async Task<IActionResult> AssignShipperVietnam([FromBody] AssignShipperDTO assignShipperDTO)
        {
            var result = await _orderStorageService.AssignShipperVietnam(assignShipperDTO);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        /*
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
        */

        [HttpGet]
        [Route("shipper/{shipperId}")]
        public async Task<IActionResult> GetOrdersByShipper(Guid shipperId)
        {
            var orders = await _orderStorageService.GetOrdersForShipper(shipperId);

            if (orders == null || !orders.Any())
            {
                return NotFound("No orders found for this shipper.");
            }

            return Ok(orders);
        }
    }
}
