using AutoMapper;
using Common.Enum;
using DAL.Entities;
using DAL.UnitOfWork;
using Service.Interfaces;

namespace Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Role?> GetCustomerRole()
        {
            var result = await _unitOfWork.Role.GetByCondition(c => c.RoleName == RoleEnum.Customer.ToString());
            return result;
        }
		public async Task<Role?> GetFarmRole()
		{
			var result = await _unitOfWork.Role.GetByCondition(c => c.RoleName == RoleEnum.KoiFarmManager.ToString());
			return result;
		}

        public async Task<Role?> GetShipperRole()
        {
            var result = await _unitOfWork.Role.GetByCondition(c => c.RoleName == RoleEnum.Shipper.ToString());
            return result;
        }

        public bool CheckUserNameExist(string userName)
        {
            var userList = _unitOfWork.User.GetAll();
            if (userList.Any(c => c.UserName == userName))
            {
                return true;
            }
            return false;
        }

        public bool CheckEmailExist(string email)
        {
            var userList = _unitOfWork.User.GetAll();
            if (userList.Any(c => c.Email == email))
            {
                return true;
            }
            return false;
        }

        public bool CheckPhoneExist(string phone)
        {
            var userList = _unitOfWork.User.GetAll();
            if (userList.Any(c => c.Phone == phone))
            {
                return true;
            }
            return false;
        }



		//public bool CheckFarmExist(string? farmName)
		//{
		//	var userList = _unitOfWork.User.GetAll();
		//	if (userList.Any(c => c.FarmName == farmName))
		//	{
		//		return true;
		//	}
		//	return false;
		//}

        public async Task<bool> CheckUserExist(Guid userId)
        {
            var user = await _unitOfWork.User.GetByCondition(c => c.UserId == userId);
            if(user != null)
            {
                return true;
            }
            return false;
        }
     
        public async Task<ShipperDto[]> GetAllShipperInStorageProvince(Guid storageProvinceId) {
            var shipperRole = await _unitOfWork.Role.GetByCondition(r => r.RoleName == RoleEnum.Shipper.ToString());
            if (shipperRole == null)
            {
                return Array.Empty<ShipperDto>();
            }

            var shippers = await _unitOfWork.User.GetAllShipperInStorageProvinceAsync(storageProvinceId, shipperRole.RoleId);
            return shippers;
        }
    }
}
