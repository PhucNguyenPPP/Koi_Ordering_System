using Common.DTO.Auth;
using Common.DTO.General;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

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

    }
}
