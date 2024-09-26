using Common.DTO.Auth;
using Common.DTO.General;
using Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IAuthService
    {
        Task<bool> SignUpCustomer(SignUpCustomerRequestDTO signUpCustomerRequestDTO);
        Task<ResponseDTO> CheckValidationSignUpCustomer(SignUpCustomerRequestDTO signUpCustomerRequestDTO);
        byte[] GenerateSalt();
        byte[] GenerateHashedPassword(string password, byte[] saltBytes);
        Task<LoginResponseDTO?> CheckLogin(LoginRequestDTO loginRequestDTO);
    }
}
