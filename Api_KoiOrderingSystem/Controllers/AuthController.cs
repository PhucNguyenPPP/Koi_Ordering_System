using Common.DTO.Auth;
using Common.DTO.General;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Api_KoiOrderingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("new-customer")]
        public async Task<IActionResult> SignUpCustomer([FromForm] SignUpCustomerRequestDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDTO(ModelState.ToString() ?? "Unknow error", 400, false, null));
            }

            var checkValid = await _authService.CheckValidationSignUpCustomer(model);
            if (!checkValid.IsSuccess)
            {
                return BadRequest(checkValid);
            }

            var signUpResult = await _authService.SignUpCustomer(model);
            if (signUpResult)
            {
                return Created("Sign up customer successfully", 
                    new ResponseDTO("Sign up customer successfully", 201, true, null));
            }
            else
            {
                return BadRequest(new ResponseDTO("Sign up customer unsuccessfully", 400, true, null));
            }
        }
		[HttpPost("new-farm")]
		public async Task<IActionResult> SignUpFarm ([FromForm] SignUpFarmRequestDTO model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new ResponseDTO(ModelState.ToString() ?? "Unknow error", 400, false, null));
			}

			var checkValid = await _authService.CheckValidationSignUpFarm(model);
			if (!checkValid.IsSuccess)
			{
				return BadRequest(checkValid);
			}

			var signUpResult = await _authService.SignUpFarm(model);
			if (signUpResult)
			{
				return Created("Sign up farm successfully",
					new ResponseDTO("Sign up farm successfully", 201, true, null));
			}
			else
			{
				return BadRequest(new ResponseDTO("Sign up farm unsuccessfully", 400, true, null));
			}
		}

        [HttpPost("new-shipper")]
        public async Task<IActionResult> SignUpShipper([FromForm] SignUpShipperRequestDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDTO(ModelState.ToString() ?? "Unknow error", 400, false, null));
            }

            var checkValid = await _authService.CheckValidationSignUpShipper(model);
            if (!checkValid.IsSuccess)
            {
                return BadRequest(checkValid);
            }

            var signUpResult = await _authService.SignUpShipper(model);
            if (signUpResult)
            {
                return Created("Sign up shipper successfully",
                    new ResponseDTO("Sign up shipper successfully", 201, true, null));
            }
            else
            {
                return BadRequest(new ResponseDTO("Sign up shipper unsuccessfully", 400, true, null));
            }
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDTO(ModelState.ToString() ?? "Unknown error", 400, false, ModelState));
            }
            var result = await _authService.CheckLogin(loginRequestDTO);
            if (result != null)
            {
                return Ok(new ResponseDTO("Sign in successfully", 200, true, result));
            }
            return BadRequest(new ResponseDTO("Sign in unsuccessfully", 400, false));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> GetNewTokenFromRefreshToken([FromBody] RequestTokenDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDTO(ModelState.ToString() ?? "Unknown error", 400, false));
            }

            var result = await _authService.RefreshAccessToken(model);
            if (result == null || string.IsNullOrEmpty(result.AccessToken))
            {
                return BadRequest(new ResponseDTO("Create refresh token unsuccessfully", 400, false, result));
            }
            return Created("Create refresh token successfully", 
                new ResponseDTO("Create refresh token successfully", 201, true, result));
        }

        [HttpGet("/user/access-token/{accessToken}")]
        public async Task<IActionResult> GetUserByToken(string accessToken)
        {
            var result = await _authService.GetUserByAccessToken(accessToken);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([Required] string rfToken)
        {

            var response = await _authService.LogOut(rfToken);

            if (response)
            {
                return Ok(new ResponseDTO("Log out successfully", 200, true));
            }

            return BadRequest(new ResponseDTO("Log out failed", 400, false));
        }

    }
}
