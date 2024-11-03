using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Common.DTO.General;
using Common.Enum;
using DAL.Entities;
using DAL.UnitOfWork;
using Service.Interfaces;

namespace Service.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public DashboardService(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userService = userService;
        }


        public async Task<ResponseDTO> GetRevenueByAdmin(DateOnly startdate, DateOnly enddate)
        {
            var orderList = _unitOfWork.Order
                .GetAllByCondition(c =>
                c.Status == OrderStatusConstant.Completed ||
                c.Status == OrderStatusConstant.CompletedRefund)
                .ToList();
            var sum = CalculateRevenue(startdate, enddate, orderList);
            return new ResponseDTO("Get revenue by time period successfully", 200, true, sum);
        }


        public async Task<ResponseDTO> GetRevenueByFarm(DateOnly startdate, DateOnly enddate, Guid farmId)
        {
            var farm = _unitOfWork.KoiFarm.GetAllByCondition(c=> c.KoiFarmId == farmId);
            if (!farm.Any())
            {
                return new ResponseDTO("Invalid koi farm!", 400, false);
            }
            var orderList = _unitOfWork.Order
                .GetAllByCondition(c=> 
                c.Kois.FirstOrDefault().FarmId == farmId && 
                c.Status == OrderStatusConstant.Completed || 
                c.Status == OrderStatusConstant.CompletedRefund).ToList();
            var sum = CalculateRevenue(startdate, enddate, orderList);
            return new ResponseDTO("Get revenue by time period successfully", 200, true, sum);
        }

        
        public async Task<ResponseDTO> GetProfitByAdmin(DateOnly startdate, DateOnly enddate)
        {
            ResponseDTO ? revenue = await GetRevenueByAdmin(startdate, enddate);
            decimal ? revenueSum = 0;
            if (revenue.IsSuccess)
            {
                revenueSum = (decimal)revenue.Result;
            }
            else
            {
                return revenue;
            }
            decimal? profit;
            profit = revenueSum * 30 / 100;
            
            return new ResponseDTO("Get profit by time range successfully", 200, true, profit);
        }

        public decimal? CalculateRevenue(DateOnly startdate, DateOnly enddate, List<Order> orderList)
        {
            DateOnly? endDate = DateOnly.MinValue;
            decimal? sum = 0;
            decimal? finalPrice = 0;
            decimal? totalPrice = 0;

            for (int i = 0; i < orderList.Count(); i++)
            {
                if (orderList[i].Status == OrderStatusConstant.Completed)
                {
                    var arrivalTime = _unitOfWork.OrderStorage
                        .GetAllByCondition(c => c.OrderId == orderList[i].OrderId)
                        .OrderByDescending(s => s.ArrivalTime)
                        .Select(c => c.ArrivalTime)
                        .FirstOrDefault();
                    endDate = arrivalTime.HasValue ?
                        DateOnly.FromDateTime(arrivalTime.Value) :
                        (DateOnly?)null;
                    if (endDate >= startdate && endDate <= enddate)
                    {
                        totalPrice = orderList[i].TotalPrice;
                        totalPrice -= decimal.Parse(orderList[i].ShippingFee);
                        sum += totalPrice;
                    }
                }
                else if (orderList[i].Status == OrderStatusConstant.CompletedRefund)
                {
                    endDate = orderList[i].RefundCompletedDate.HasValue ?
                        DateOnly.FromDateTime(orderList[i].RefundCompletedDate.Value) :
                        (DateOnly?)null;
                    if (endDate >= startdate && endDate <= enddate)
                    {
                        totalPrice = orderList[i].TotalPrice;
                        finalPrice = totalPrice - (totalPrice * orderList[i].RefundPercentage / 100) - decimal.Parse(orderList[i].ShippingFee);
                        sum += finalPrice;
                    }
                }
            }
            return sum;
        }
    }
}
