using Common.DTO.General;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IUserService
    {
        Task<Role?> GetCustomerRole();
		Task<Role?> GetFarmRole();

        Task<Role?> GetShipperRole();

        bool CheckUserNameExist(string userName);
        bool CheckEmailExist(string email);
        bool CheckPhoneExist(string phone);
		//bool CheckFarmExist(string? farmName);
        Task<bool> CheckUserExist (Guid userId);
        Task<ShipperDto[]> GetAllShipperInStorageProvince(Guid storageProvinceId);
	}
}
