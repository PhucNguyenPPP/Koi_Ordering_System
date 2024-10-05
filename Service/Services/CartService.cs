using AutoMapper;
using Common.DTO.Cart;
using Common.DTO.General;
using DAL.Entities;
using DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IKoiService _koiService;
        public CartService(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService,
            IKoiService koiService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
            _koiService = koiService;

        }
        public async Task<bool> AddKoiToCart(CartDTO cartDTO)
        {
            Cart newCart = new Cart()
            {
                CartId = Guid.NewGuid(),
                KoiId = cartDTO.KoiId,
                UserId = cartDTO.UserId,
                CreatedDate = DateTime.Now,
            };
            await _unitOfWork.Cart.AddAsync(newCart);
            return await _unitOfWork.SaveChangeAsync();
        }

        public async Task<ResponseDTO> CheckValidationCart(CartDTO cartDTO)
        {
            var userExist = await _userService.CheckUserExist(cartDTO.UserId);
            if (!userExist)
            {
                return new ResponseDTO("User does not exist", 404, false);
            }

            var koiExist = await _koiService.CheckKoiExist(cartDTO.KoiId);
            if (!koiExist)
            {
                return new ResponseDTO("Koi does not exist", 404, false);
            }

            var koiCartExist = await _unitOfWork.Cart.GetByCondition(a => a.KoiId == cartDTO.KoiId && a.UserId == cartDTO.UserId);
            if(koiCartExist != null)
            {
                return new ResponseDTO("Koi already added into cart", 400, false);
            }
            return new ResponseDTO("Check successfully", 200, true); ;
        }

        public async Task<ResponseDTO> CheckCartExist(Guid cartId)
        {
            var cartExist = await _unitOfWork.Cart.GetByCondition(a => a.CartId == cartId);
            if(cartExist == null)
            {
                return new ResponseDTO("CartId is not exist", 404, false);
            } else
            {
                return new ResponseDTO("Check successfully", 200, true);
            }
        }

        public async Task<bool> DeleteCart(Guid cartId)
        {
            var cartItem = await _unitOfWork.Cart.GetByCondition(a => a.CartId == cartId);
            _unitOfWork.Cart.Delete(cartItem);
            var result = await _unitOfWork.SaveChangeAsync();
            return result;
        }

        public List<GetCartDTO>? GetCartByUser(Guid userId)
        {
            var cartList = _unitOfWork.Cart.GetAllByCondition(a => a.UserId == userId)
                .Include(c => c.Koi)
                .ThenInclude(c => c.Farm)
                .OrderByDescending(c => c.CreatedDate);
            if (cartList == null)
            {
                return null;
            }
            List<GetCartDTO> cart = _mapper.Map<List<GetCartDTO>>(cartList);

            return cart;

        }
    }
}
