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
        bool CheckUserNameExist(string userName);
        bool CheckEmailExist(string email);
        bool CheckPhoneExist(string phone);
		Task<ResponseDTO> GetFarmDetail(Guid userId);
	}
}
