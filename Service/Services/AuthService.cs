using AutoMapper;
using Common.Constant;
using Common.DTO.Auth;
using Common.DTO.General;
using Common.Enum;
using DAL.Entities;
using DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IImageService _imageService;
        private readonly IConfiguration _config;

        public AuthService(IMapper mapper, IUserService userService,
            IUnitOfWork unitOfWork, IImageService imageService,
            IConfiguration config)
        {
            _mapper = mapper;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _imageService = imageService;
            _config = config;
        }

        public async Task<bool> SignUpCustomer(SignUpCustomerRequestDTO model)
        {
            var customer = _mapper.Map<User>(model);
            var role = await _userService.GetCustomerRole();
            if (role == null)
            {
                return false;
            }
            var salt = GenerateSalt();
            var passwordHash = GenerateHashedPassword(model.Password, salt);
            var avatarLink = await _imageService.StoreImageAndGetLink(model.AvatarLink, FileNameFirebaseStorage.UserImage);

            customer.UserId = Guid.NewGuid();
            customer.RoleId = role.RoleId;
            customer.Salt = salt;
            customer.PasswordHash = passwordHash;
            customer.AvatarLink = avatarLink;
            customer.Status = true;

            await _unitOfWork.User.AddAsync(customer);
            return await _unitOfWork.SaveChangeAsync();
        }

        public byte[] GenerateSalt()
        {
            byte[] saltBytes = new byte[32];
            var rng = RandomNumberGenerator.Create();
            rng.GetNonZeroBytes(saltBytes);
            return saltBytes;
        }

        public byte[] GenerateHashedPassword(string password, byte[] saltBytes)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] passwordWithSaltBytes = new byte[passwordBytes.Length + saltBytes.Length];

            for (int i = 0; i < passwordBytes.Length; i++)
            {
                passwordWithSaltBytes[i] = passwordBytes[i];
            }

            for (int i = 0; i < saltBytes.Length; i++)
            {
                passwordWithSaltBytes[passwordBytes.Length + i] = saltBytes[i];
            }

            var cryptoProvider = SHA512.Create();
            byte[] hashedBytes = cryptoProvider.ComputeHash(passwordWithSaltBytes);

            return hashedBytes;
        }

        public async Task<ResponseDTO> CheckValidationSignUpCustomer(SignUpCustomerRequestDTO model)
        {
            if (model.DateOfBirth >= DateTime.Now)
            {
                return new ResponseDTO("Date of birth is invalid", 400, false);
            }

            if (model.Gender != GenderEnum.Male.ToString() 
                && model.Gender != GenderEnum.Female.ToString()
                && model.Gender != GenderEnum.Other.ToString())
            {
                return new ResponseDTO("Gender is invalid", 400, false);
            }

            var checkUserNameExist = _userService.CheckUserNameExist(model.UserName);
            if (checkUserNameExist)
            {
                return new ResponseDTO("Username already exists", 400, false);
            }

            var checkEmailExist = _userService.CheckEmailExist(model.Email);
            if (checkEmailExist)
            {
                return new ResponseDTO("Email already exists", 400, false);
            }

            var checkPhoneExist = _userService.CheckPhoneExist(model.Phone);
            if (checkPhoneExist)
            {
                return new ResponseDTO("Phone already exists", 400, false);
            }

            return new ResponseDTO("Check successfully", 200, true);
        }

        public async Task<LoginResponseDTO?> CheckLogin(LoginRequestDTO loginRequestDTO)
        {
            var user = _unitOfWork.User.GetAllByCondition(x => x.UserName == loginRequestDTO.UserName)
                .Include(u => u.Role).FirstOrDefault();

            if (user == null)
            {
                return null;
            }

            if (VerifyPasswordHash(loginRequestDTO.Password, user.PasswordHash, user.Salt))
            {
                string jwtTokenId = $"JTI{Guid.NewGuid()}";

                string refreshToken = await CreateNewRefreshToken(user.UserId, jwtTokenId);

                var refreshTokenValid = _unitOfWork.RefreshToken
                    .GetAllByCondition(a => a.UserId == user.UserId
                    && a.RefreshToken1 != refreshToken)
                    .ToList();

                foreach (var token in refreshTokenValid)
                {
                    token.IsValid = false;
                }

                _unitOfWork.RefreshToken.UpdateRange(refreshTokenValid);
                await _unitOfWork.SaveChangeAsync();

                var accessToken = CreateToken(user, jwtTokenId);

                return new LoginResponseDTO
                {
                    AccessToken = accessToken,
                    User = _mapper.Map<LocalUserDTO>(user),
                    RefreshToken = refreshToken
                };
            };
            return null;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHashDb, byte[] salt)
        {
            var passwordHash = GenerateHashedPassword(password, salt);
            bool areEqual = passwordHashDb.SequenceEqual(passwordHash);
            return areEqual;
        }

        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }

        private async Task<string> CreateNewRefreshToken(Guid userId, string jwtId)
        {
            RefreshToken refreshAccessToken = new()
            {
                RefreshTokenId = Guid.NewGuid(),
                UserId = userId,
                JwtId = jwtId,
                ExpiredAt = DateTime.Now.AddHours(24),
                IsValid = true,
                RefreshToken1 = CreateRandomToken(),
            };
            await _unitOfWork.RefreshToken.AddAsync(refreshAccessToken);
            await _unitOfWork.SaveChangeAsync();
            return refreshAccessToken.RefreshToken1;
        }

        private string CreateToken(User user, string jwtId)
        {
            var roleName = user.Role.RoleName;


            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName.ToString()),
                new Claim(ClaimTypes.Role, roleName.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, jwtId),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, DateTime.Now.AddSeconds(15).ToString(), ClaimValueTypes.Integer64)
            };

            var key = _config.GetSection("ApiSetting")["Secret"];
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(key ?? ""));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
               claims: claims,
               expires: DateTime.Now.AddMinutes(15),
               signingCredentials: credentials
           );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
