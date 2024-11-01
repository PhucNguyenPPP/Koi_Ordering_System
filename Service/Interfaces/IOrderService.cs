using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.DTO.General;
using Common.DTO.KoiFish;
using Common.DTO.Order;

namespace Service.Interfaces
{
    public interface IOrderService
    {
        Task<ResponseDTO> CheckValidationCreateOrder(CreateOrderDTO createOrderDTO);
        Task<ResponseDTO> CreateOrder(CreateOrderDTO createOrderDTO);
        Task<bool> CheckOrderExist(Guid orderId);
        Task<bool> UpdateOrderPackaging(Guid orderId, UpdateOrderPackagingRequest request);
        Task<bool> UpdateOrderCompleted(Guid orderId);
        Task<ResponseDTO> GetAllHistoryOrder(Guid customerId);
        Task<ResponseDTO> GetAllFarmHistoryOrder(Guid farmId);
        Task<ResponseDTO> GetAllStorageHistoryOrder(Guid storageProvinceId);
        Task<ResponseDTO> GetOrderDetail(Guid orderId);
        Task<ResponseDTO> AssignFlightToOrder(AssignFlightToOrderDTO assignFlightToOrderDTO);
        Task<ResponseDTO> GetAllRefundOrder();
    }
}
