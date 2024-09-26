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
                return Ok(new ResponseDTO("Đăng nhập thành công", 200, true, result));
            }
            return BadRequest(new ResponseDTO("Đăng nhập thất bại", 400, false));
        }

    }
}
