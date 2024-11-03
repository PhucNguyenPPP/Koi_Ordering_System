using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.DTO.General;

namespace Service.Interfaces
{
    public interface IDashboardService
    {
        Task<ResponseDTO> GetRevenueByAdmin(DateOnly startdate, DateOnly enddate);
        Task<ResponseDTO> GetRevenueByFarm(DateOnly startdate, DateOnly enddate, Guid farmId);
        Task<ResponseDTO> GetProfitByAdmin(DateOnly startdate, DateOnly enddate);
        Task<ResponseDTO> GetProfitOfAdminByYear(int year);
        Task<ResponseDTO> GetProfitOfFarmByYear(int year, Guid farmId);
    }
}
