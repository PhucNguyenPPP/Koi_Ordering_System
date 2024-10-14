using Common.DTO.General;
using Common.DTO.Payment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Service.Interface;
using Service.Interfaces;

namespace Api_Ace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;

        public PaymentController(IOrderService orderService, 
            IPaymentService paymentService)
        {
            _orderService = orderService;
            _paymentService = paymentService;
        }
        [HttpPost("vnpay-payment")]
        public async Task<IActionResult> CreatePaymentUrl(Guid orderId)
        {
            var checkExist = await _orderService.CheckOrderExist(orderId);
            if (!checkExist)
            {
                return BadRequest(new ResponseDTO("Order does not exist", 400, false));
            }

            var result = await _paymentService.CreatePaymentVNPayRequest(orderId, HttpContext);
            if (result.IsNullOrEmpty())
            {
                return BadRequest(new ResponseDTO("Create payement url failed", 400, false));
            }

            return Created("Create payement url success", 
                new ResponseDTO("Create payment url success", 201, true, result));
        }

        [HttpPut("response-payment")]
        public async Task<IActionResult> HandleResponseVnPay([FromBody] VnPayResponseDTO responseInfo)
        {
            if (!ModelState.IsValid)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ResponseDTO(ModelState.ToString()!, 400, false, null));
                }
            }

            var response = await _paymentService.HandlePaymentResponse(responseInfo);

            if (response)
            {
                return Ok(new ResponseDTO("Handle successfully", 201, true, response));
            }

            return StatusCode(500, new ResponseDTO("Handle failed", 500, false, null));
        }
    }
}
