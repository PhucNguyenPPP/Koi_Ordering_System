using System.ComponentModel.DataAnnotations;
using Common.DTO.General;
using Common.DTO.KoiFish;
using Common.DTO.Order;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Service.Interfaces;
using Service.Services;

namespace Api_KoiOrderingSystem.Controllers
{
    [Route("odata")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService order)
        {
            _orderService = order;
        }

        [HttpPost("order")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDTO(ModelState.ToString() ?? "Unknow error", 400, false, null));
            }

            var checkValid = await _orderService.CheckValidationCreateOrder(model);
            if (!checkValid.IsSuccess)
            {
                return BadRequest(checkValid);
            }

            var signUpResult = await _orderService.CreateOrder(model);
            if (signUpResult.IsSuccess)
            {
                return Created("Success", signUpResult);
            }
            else
            {
                return BadRequest(signUpResult);
            }
        }

        [HttpPut("packaging/{orderId}")]
        public async Task<IActionResult> UpdateOrderPackaging(Guid orderId, [FromBody] UpdateOrderPackagingRequest request)
        {
            var result = await _orderService.UpdateOrderPackaging(orderId, request);
            
            if (!result)
            {
                return NotFound("Order not found.");
            }

            return Ok("Order updated successfully.");
        }

        [HttpGet("all-history-order")]
        [EnableQuery]
        public async Task<IActionResult> GetAllHistoryOrder([Required]Guid userId)
        {
            ResponseDTO responseDTO = await _orderService.GetAllHistoryOrder(userId);
            if (responseDTO.IsSuccess == false)
            {
                if (responseDTO.StatusCode == 404)
                {
                    return NotFound(responseDTO);
                }
                return BadRequest(responseDTO);

            }
            return Ok(responseDTO.Result);
        }
    }
}
