using AutoMapper;
using Common.Constant;
using Common.DTO.Auth;
using Common.DTO.General;
using Common.Enum;
using DAL.Entities;
using DAL.UnitOfWork;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public AuthService(IMapper mapper, IUserService userService,
            IUnitOfWork unitOfWork, IImageService imageService)
        {
            _mapper = mapper;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _imageService = imageService;

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
    }
}
