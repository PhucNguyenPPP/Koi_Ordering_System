using AutoMapper;
using Common.Constant;
using Common.DTO.Auth;
using Common.DTO.General;
using Common.Enum;
using DAL.Entities;
using DAL.Interfaces;
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
        private readonly IStorageProvinceService _storageProvinceService;
		private readonly IKoiFarmService _koiFarmService;

		public AuthService(IMapper mapper, IUserService userService,
            IUnitOfWork unitOfWork, IImageService imageService,
            IStorageProvinceService storageProvinceService,
            IKoiFarmService koiFarmService,IConfiguration config)
        {
            _mapper = mapper;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _imageService = imageService;
            _storageProvinceService = storageProvinceService;
            _koiFarmService = koiFarmService;
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
                .Include(c => c.KoiFarm)
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
                new Claim(JwtRegisteredClaimNames.Exp, DateTime.Now.AddMinutes(30).ToString(), ClaimValueTypes.Integer64)
            };

            var key = _config.GetSection("ApiSetting")["Secret"];
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(key ?? ""));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
               claims: claims,
               expires: DateTime.Now.AddMinutes(30),
               signingCredentials: credentials
           );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<TokenDTO> RefreshAccessToken(RequestTokenDTO model)
        {
            // Find an existing refresh token
            var existingRefreshToken = _unitOfWork.RefreshToken
                .GetAllByCondition(r => r.RefreshToken1 == model.RefreshToken)
                .FirstOrDefault();

            if (existingRefreshToken == null)
            {
                return new TokenDTO()
                {
                    Message = "Token is not exists"
                };
            }

            // Compare data from exixsting refresh and access token provided and if there is any missmatch then consider it as fraud
            var isTokenValid = GetAccessTokenData(model.AccessToken, existingRefreshToken.UserId, existingRefreshToken.JwtId);
            if (!isTokenValid)
            {
                existingRefreshToken.IsValid = false;
                await _unitOfWork.SaveChangeAsync();
                return new TokenDTO()
                {
                    Message = "Token is invalid"
                };
            }

            // Check accessToken expire ?
            var tokenHandler = new JwtSecurityTokenHandler();
            var test = tokenHandler.ReadJwtToken(model.AccessToken);
            if (test == null) return new TokenDTO()
            {
                Message = "Error when creating token"
            };

            var accessExpiredDateTime = test.ValidTo;
            // Sử dụng accessExpiredDateTime làm giá trị thời gian hết hạn

            if (accessExpiredDateTime > DateTime.UtcNow)
            {
                return new TokenDTO()
                {
                    Message = "Token is not expired"
                };
            }
            // When someone tries to use not valid refresh token, fraud possible

            if (!existingRefreshToken.IsValid)
            {
                var chainRecords = _unitOfWork.RefreshToken
                    .GetAllByCondition(u => u.UserId == existingRefreshToken.UserId
                    && u.JwtId == existingRefreshToken.JwtId)
                    .ToList();

                foreach (var item in chainRecords)
                {
                    item.IsValid = false;
                }
                _unitOfWork.RefreshToken.UpdateRange(chainRecords);
                await _unitOfWork.SaveChangeAsync();
                return new TokenDTO
                {
                    Message = "Token is invalid"
                };
            }

            // If it just expired then mark as invalid and return empty

            if (existingRefreshToken.ExpiredAt < DateTime.Now)
            {
                existingRefreshToken.IsValid = false;
                await _unitOfWork.SaveChangeAsync();
                return new TokenDTO()
                {
                    Message = "Token is expired"
                };
            }

            // Replace old refresh with a new one with updated expired date
            var newRefreshToken = await ReNewRefreshToken(existingRefreshToken.UserId,
                existingRefreshToken.JwtId);

            // Revoke existing refresh token
            existingRefreshToken.IsValid = false;
            await _unitOfWork.SaveChangeAsync();
            // Generate new access token
            var user = _unitOfWork.User.GetAllByCondition(a => a.UserId == existingRefreshToken.UserId)
                .Include(u => u.Role)
                .Include(c => c.KoiFarm)
                .FirstOrDefault();

            if (user == null)
            {
                return new TokenDTO();
            }

            var newAccessToken = CreateToken(user, existingRefreshToken.JwtId);

            return new TokenDTO()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Message = "Create token successfully"
            };
        }

        private bool GetAccessTokenData(string accessToken, Guid expectedUserId, string expectedTokenId)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.ReadJwtToken(accessToken);
                var jwtId = jwt.Claims.FirstOrDefault(a => a.Type == JwtRegisteredClaimNames.Jti)?.Value;
                var userId = jwt.Claims.FirstOrDefault(a => a.Type == JwtRegisteredClaimNames.Sub)?.Value;
                userId = userId ?? string.Empty;
                return Guid.Parse(userId) == expectedUserId && jwtId == expectedTokenId;
            }
            catch
            {
                return false;
            }
        }

        private async Task<string> ReNewRefreshToken(Guid userId, string jwtId)
        {
            var time = _unitOfWork.RefreshToken.GetAllByCondition(a => a.JwtId == jwtId)
                .FirstOrDefault();
            RefreshToken refreshAccessToken = new()
            {
                RefreshTokenId = Guid.NewGuid(),
                UserId = userId,
                JwtId = jwtId,
                ExpiredAt = time?.ExpiredAt != null ? time.ExpiredAt : DateTime.Now.AddHours(24),
                IsValid = true,
                RefreshToken1 = CreateRandomToken(),
            };
            await _unitOfWork.RefreshToken.AddAsync(refreshAccessToken);
            await _unitOfWork.SaveChangeAsync();
            return refreshAccessToken.RefreshToken1;
        }

        public async Task<ResponseDTO> GetUserByAccessToken(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return new ResponseDTO("Token is empty", 400, false);
            }
            var userId = ExtractUserIdFromToken(accessToken);
            if (userId == "Token is expired")
            {
                return new ResponseDTO("Token is expired", 400, false);
            }
            if (userId == "Token is invalid")
            {
                return new ResponseDTO("Token is invalid", 400, false);
            }
            var user = await _unitOfWork.User.GetAllByCondition(c => c.UserId.ToString() == userId
            && c.Status == true).Include(c => c.Role).Include(c => c.KoiFarm).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ResponseDTO("User not found", 400, false);
            }
            var mapUser = _mapper.Map<LocalUserDTO>(user);
            return new ResponseDTO("Get user by token successfully", 200, true, mapUser);
        }

        private string ExtractUserIdFromToken(string accessToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(accessToken);

                //token expired
                var expiration = jwtToken.ValidTo;

                if (expiration < DateTime.UtcNow)
                {
                    return "Token is expired";
                }
                else
                {
                    string userId = jwtToken.Subject;
                    return userId;
                }
            }
            catch (Exception ex)
            {
                return "Token is invalid";
            }
        }

        public async Task<ResponseDTO> CheckValidationSignUpFarm(SignUpFarmRequestDTO model)
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
            var checkFarmExist = _koiFarmService.CheckFarmExist(model.FarmName);
            if (checkFarmExist)
            {
                return new ResponseDTO("Farm already exists", 400, false);
            }
            var checkValidJapanStorageProvince = await _storageProvinceService.CheckJapanStorageProvince(model.StorageProvinceId);
            if (!checkValidJapanStorageProvince.IsSuccess)
            {
                return checkValidJapanStorageProvince;
            }
            return new ResponseDTO("Check successfully", 200, true);
        }

        public async Task<bool> SignUpFarm(SignUpFarmRequestDTO model)
        {
            var user = _mapper.Map<User>(model);
            var farm = _mapper.Map<KoiFarm>(model); 
            var role = await _userService.GetFarmRole();
            if (role == null)
            {
                return false;
            }
            var salt = GenerateSalt();
            var passwordHash = GenerateHashedPassword(model.Password, salt);
            var avatarLink = await _imageService.StoreImageAndGetLink(model.AvatarLink, FileNameFirebaseStorage.UserImage);
            user.UserId = Guid.NewGuid();
			user.RoleId = role.RoleId;
			user.Salt = salt;
			user.PasswordHash = passwordHash;
			user.AvatarLink = avatarLink;
			user.Status = true;
            farm.KoiFarmId= Guid.NewGuid();
            farm.KoiFarmManagerId = user.UserId;
            farm.FarmAvatar=user.AvatarLink;
			await _unitOfWork.User.AddAsync(user);
            await _unitOfWork.KoiFarm.AddAsync(farm);
            return await _unitOfWork.SaveChangeAsync();
        }

        public async Task<ResponseDTO> CheckValidationSignUpShipper(SignUpShipperRequestDTO model)
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

        public async Task<bool> SignUpShipper(SignUpShipperRequestDTO model)
        {
            var shipper = _mapper.Map<User>(model);
            var role = await _userService.GetShipperRole();
            if (role == null)
            {
                return false;
            }
            var salt = GenerateSalt();
            var passwordHash = GenerateHashedPassword(model.Password, salt);
            var avatarLink = await _imageService.StoreImageAndGetLink(model.AvatarLink, FileNameFirebaseStorage.UserImage);

            shipper.UserId = Guid.NewGuid();
            shipper.RoleId = role.RoleId;
            shipper.Salt = salt;
            shipper.PasswordHash = passwordHash;
            shipper.AvatarLink = avatarLink;
            shipper.Status = true;

            await _unitOfWork.User.AddAsync(shipper);
            return await _unitOfWork.SaveChangeAsync();
        }
    }
}
