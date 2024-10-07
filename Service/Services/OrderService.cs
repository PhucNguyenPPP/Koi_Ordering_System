using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Common.DTO.General;
using Common.DTO.Order;
using Common.Enum;
using DAL.Entities;
using DAL.UnitOfWork;
using Microsoft.IdentityModel.Tokens;
using Service.Interfaces;

namespace Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public OrderService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDTO> CheckValidationCreateOrder(CreateOrderDTO createOrderDTO) 
        {
            var customer = _unitOfWork.User.GetAllByCondition(c => c.UserId == createOrderDTO.CustomerId && c.Role.RoleName.Equals(RoleEnum.KoiFarm));
            if (customer.IsNullOrEmpty()) return new ResponseDTO("Customer not exist!", 400, false);
            
            for(int i = 0; i < createOrderDTO.KoiId.Count; i++)
            {
                var koi = _unitOfWork.Koi.GetAllByCondition(c => c.KoiId == createOrderDTO.KoiId[i]);
                if (koi.IsNullOrEmpty()) return new ResponseDTO("Koi not exist!", 400, false);
                
            }

            if(createOrderDTO.Length < 1) return new ResponseDTO("Invalid length!", 400, false);

            if (createOrderDTO.Width < 1) return new ResponseDTO("Invalid width!", 400, false);

            if (createOrderDTO.Height < 1) return new ResponseDTO("Invalid Height!", 400, false);

            if (createOrderDTO.Weight < 1) return new ResponseDTO("Invalid Weight!", 400, false);



            return new ResponseDTO("Check validation successfully!", 200, true);
        } 

        public async Task<ResponseDTO> CreateOrder(CreateOrderDTO createOrderDTO)
        {

        }
    }
}
