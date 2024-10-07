using Common.DTO.General;
using Common.DTO.KoiFish;
using Common.DTO.Order;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Services;

namespace Api_KoiOrderingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService order)
        {
            _orderService = order;
        }

        [HttpPost("koi")]
        public async Task<IActionResult> CreateOrder([FromForm] CreateOrderDTO model)
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
            if (signUpResult)
            {
                return Created("Success", new ResponseDTO("Create order successfully!", 201, true, null));
            }
            else
            {
                return BadRequest(new ResponseDTO("Create order failed!", 400, true, null));
            }
        }
    }
}
