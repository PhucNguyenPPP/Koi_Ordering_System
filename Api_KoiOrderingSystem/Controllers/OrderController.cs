using System.ComponentModel.DataAnnotations;
using Common.DTO.General;
using Common.DTO.KoiFish;
using Common.DTO.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Service.Interfaces;
using Service.Services;

namespace Api_KoiOrderingSystem.Controllers
{
    [Route("odata")]
    [ApiController]
    public class OrderController : ODataController
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService order)
        {
            _orderService = order;
        }

        [HttpPost("order")]
        [Authorize(Roles = "Customer")]
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

        [HttpPut("completed/{orderId}")]
        public async Task<IActionResult> UpdateOrderCompleted(Guid orderId)
        {
            var result = await _orderService.UpdateOrderCompleted(orderId);
            
            if (!result)
            {
                return NotFound("Order not found.");
            }

            return Ok("Order updated successfully.");
        }

        [HttpGet("all-customer-history-order")]
        [Authorize(Roles = "Customer")]
        [EnableQuery]
        public async Task<IActionResult> GetAllHistoryOrder([Required]Guid customerId)
        {
            ResponseDTO responseDTO = await _orderService.GetAllHistoryOrder(customerId);
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

        [HttpGet("all-farm-history-order")]
        [Authorize(Roles = "KoiFarmManager")]
        [EnableQuery]
        public async Task<IActionResult> GetAllFarmHistoryOrder([Required] Guid farmId)
        {
            ResponseDTO responseDTO = await _orderService.GetAllFarmHistoryOrder(farmId);
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

        [HttpGet("all-storage-history-order")]
        [Authorize(Roles = "StorageManager")]
        [EnableQuery]
        public async Task<IActionResult> GetAllStorageHistoryOrder([Required] Guid storageProvinceId)
        {
            ResponseDTO responseDTO = await _orderService.GetAllStorageHistoryOrder(storageProvinceId);
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

        [HttpGet("order-detail")]
        [Authorize(Roles = "Customer,KoiFarmManager,StorageManager,Shipper,Staff,Admin")]
        public async Task<IActionResult> GetOrderDetail([Required] Guid orderId)
        {
            ResponseDTO responseDTO = await _orderService.GetOrderDetail(orderId);
            if (responseDTO.IsSuccess == false)
            {
                if (responseDTO.StatusCode == 404)
                {
                    return NotFound(responseDTO);
                }
                return BadRequest(responseDTO);

            }
            return Ok(responseDTO);
        }
        [HttpPut("flight-of-order")]
        public async Task<IActionResult> AssignFlightToOrder([FromBody] AssignFlightToOrderDTO assignFlightToOrderDTO)
        {
            ResponseDTO responseDTO = await _orderService.AssignFlightToOrder(assignFlightToOrderDTO);
            if (responseDTO.IsSuccess == false)
            {
                if (responseDTO.StatusCode == 404)
                {
                    return NotFound(responseDTO);
                }
                return BadRequest(responseDTO);

            }
            return Ok(responseDTO);
        }

        [HttpGet("order-refund")]
        [EnableQuery]
        //[Authorize(Roles = "Customer,KoiFarmManager,StorageManager,Shipper,Staff,Admin")]
        public async Task<IActionResult> GetRefundOrder()
        {
            ResponseDTO responseDTO = await _orderService.GetAllRefundOrder();
            if (responseDTO.IsSuccess == false)
            {
                if (responseDTO.StatusCode == 404)
                {
                    return NotFound(responseDTO);
                }
                return BadRequest(responseDTO);

            }
            return Ok(responseDTO);
        }

        [HttpPost("create-refund")]
        [Authorize(Roles = "Customer,KoiFarmManager,StorageManager,Shipper,Staff,Admin")]
        public async Task<IActionResult> CreateRefundRequestOrder([FromForm] CreateRefundRequestDTO createRefundRequestDTO)
        {
            ResponseDTO responseDTO = await _orderService.CreateRefundRequestOrder(createRefundRequestDTO);
            if (responseDTO.IsSuccess == false)
            {
                if (responseDTO.StatusCode == 404)
                {
                    return NotFound(responseDTO);
                }
                return BadRequest(responseDTO);
            }
            return Ok(responseDTO);
        }

        [HttpPost("process-refund")]
        [Authorize(Roles = "Customer,KoiFarmManager,StorageManager,Shipper,Staff,Admin")]
        public async Task<IActionResult> ProcessRefundRequestOrder([FromBody] ProcessRefundRequestDTO processRefundRequestDTO)
        {
            ResponseDTO responseDTO = await _orderService.ProcessRefundRequestOrder(processRefundRequestDTO);
            if (responseDTO.IsSuccess == false)
            {
                if (responseDTO.StatusCode == 404)
                {
                    return NotFound(responseDTO);
                }
                return BadRequest(responseDTO);
            }
            return Ok(responseDTO);
        }

        [HttpPost("complete-refund")]
        public async Task<IActionResult> CompleteRefundRequestOrder([FromBody] CompleteRefundRequestDTO completeRefundRequestDTO)
        {
            ResponseDTO responseDTO = await _orderService.CompleteRefundRequestOrder(completeRefundRequestDTO.OrderId);
            if (!responseDTO.IsSuccess)
            {
                if (responseDTO.StatusCode == 404)
                {
                    return NotFound(responseDTO);
                }
                return BadRequest(responseDTO);
            }
            return Ok(responseDTO);
        }
    }
}
