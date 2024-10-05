using Common.DTO.Cart;
using Common.DTO.General;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using System.Collections.Generic;

namespace Api_KoiOrderingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IUserService _userService;

        public CartController(ICartService cartService, IUserService userService)
        {
            _cartService = cartService;
            _userService = userService;
        }

        [HttpGet("cart-user")]
        public async Task<IActionResult> GetCartOfUser(Guid userId)
        {
            var userExist = await _userService.CheckUserExist(userId);
            if (!userExist)
            {
                return NotFound(new ResponseDTO("User does not exist", 404, false));
            }
            var list = _cartService.GetCartByUser(userId);
            if (list == null)
            {
                return NotFound(new ResponseDTO("User does not have any koi in cart", 404, false));
            }
            return Ok(new ResponseDTO("Get cart of user successfully", 200, true, list));
        }

        [HttpPost("cart-user")]
        public async Task<IActionResult> AddCart(CartDTO model)
        {
            var check = await _cartService.CheckValidationCart(model);
            if (!check.IsSuccess && check.StatusCode == 404)
            {
                return NotFound(check);
            }
            else if (!check.IsSuccess && check.StatusCode == 400)
            {
                return BadRequest(check);
            }

            var result = await _cartService.AddKoiToCart(model);
            if (result)
            {
                return Created("Add into cart successfully", new ResponseDTO("Add into cart successfully", 201, true));
            }
            return BadRequest(new ResponseDTO("Add into cart unsuccessfully", 400, false));
        }

        [HttpDelete("cart-user")]
        public async Task<IActionResult> DeleteCartUser(Guid cartId)
        {
            var result = await _cartService.DeleteCart(cartId);
            if (result)
            {
                return Ok(new ResponseDTO("Delete cart successfully", 200, true));
            }
            return BadRequest(new ResponseDTO("Delete cart unsuccessfully", 400, false));

        }
    }
}
