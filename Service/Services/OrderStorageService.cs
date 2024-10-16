using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Common.DTO.General;
using Common.DTO.OrderStorage;
using Common.Enum;
using DAL.Entities;
using DAL.Interfaces;
using DAL.UnitOfWork;
using Service.Interfaces;

namespace Service.Services
{
    public class OrderStorageService : IOrderStorageService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public OrderStorageService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDTO> AssignShipper(AssignShipperDTO assignShipperDTO)
        {
            var update = false;
            var orderStorage = await CheckOrderStorageExist(assignShipperDTO.OrderStorageId);
            if (orderStorage.ShipperId != Guid.Empty)
            {
                update = true;
            }
            if (orderStorage == null)
            {
                return new ResponseDTO("OrderStorage does not exist", 400, false);
            }
            if(!orderStorage.Status)
            {
                return new ResponseDTO("Status of orderStorage is false", 400, false);
            }    
            var checkOrderStatus = await CheckOrderStatusToAssignAsync(orderStorage.OrderId);
            if (!checkOrderStatus)
            {
                return new ResponseDTO("Cannot Assign this order", 400, false);
            }
            var checkShipperProvince = await CheckShipperProvince(assignShipperDTO.ShipperId, orderStorage.StorageProvinceId, orderStorage.OrderId);
            if (!checkShipperProvince.IsSuccess)
            {
                return checkShipperProvince;
            }
            orderStorage.ShipperId = assignShipperDTO.ShipperId;
            _unitOfWork.OrderStorage.Update(orderStorage);
            var saveChanges = await _unitOfWork.SaveChangeAsync();
            if (saveChanges)
            {
                var order = await _unitOfWork.Order.GetByCondition(o => o.OrderId == orderStorage.OrderId);
                order.Status = OrderStatusConstant.ToShip;
                _unitOfWork.Order.Update(order);
                if (!update)
                {
                    return new ResponseDTO("Assign shipper sucessfully", 200, true);
                }
                return new ResponseDTO("Update shipper for order sucessfully", 200, true);
            }
            return new ResponseDTO("Assign shipper failed", 500, true);
        }

        private async Task<ResponseDTO> CheckShipperProvince(Guid shipperId, Guid storageProvinceId, Guid orderId)
        {
            var shipper = await _unitOfWork.User.GetByCondition(u => u.UserId == shipperId);
            if (!storageProvinceId.Equals(shipper.StorageProvinceId))
            {
                return new ResponseDTO("Storage Manager cannot assign this shipper", 400, false);
            }
            var order = await _unitOfWork.Order.GetByCondition(o => o.OrderId == orderId);
            var storageProvince = await _unitOfWork.StorageProvince.GetByCondition(s => s.StorageProvinceId.Equals(storageProvinceId));
            if (order.Status.Equals(OrderStatusConstant.ArrivalVietNamAirport) || order.Status.Equals(OrderStatusConstant.ArrivalVietnamStorage))
            {
                if (storageProvince.Country.Equals(StorageCountryEnum.Japan))
                {
                    return new ResponseDTO("Storage in JP cannot assign the order which had arrived VN", 400, false);
                }
            }
            return new ResponseDTO("Check Shipper and Province successfully", 200, true);
        }

        private async Task<bool> CheckOrderStatusToAssignAsync(Guid orderId)
        {
            var order = await _unitOfWork.Order.GetByCondition(o => o.OrderId.Equals(orderId));
            if (order.Status.Equals(OrderStatusConstant.Processing) || order.Status.Equals(OrderStatusConstant.ArrivalVietNamAirport) || order.Status.Equals(OrderStatusConstant.ArrivalVietnamStorage) || order.Status.Equals(OrderStatusConstant.ArrivalJapanStorage))
            {
                return true;
            }
            return false;
        }
        private Task<OrderStorage>? CheckOrderStorageExist(Guid orderStorageId)
        {
            var orderStorage = _unitOfWork.OrderStorage.GetByCondition(o => o.OrderStorageId == orderStorageId && o.Status == true);
            return orderStorage;
        }

        public async Task<ResponseDTO> ConfirmDelivery(ConfirmDeliveryDTO confirmDeliveryDTO)
        {
            var orderStorage = await CheckOrderStorageExist(confirmDeliveryDTO.OrderStorageId);
            if (orderStorage == null)
            {
                return new ResponseDTO("OrderStorage does not exist", 400, false);
            }
            if(!confirmDeliveryDTO.ShipperId.Equals(orderStorage.ShipperId))
            {
                return new ResponseDTO("You are not the shipper of this order storage", 400, false);
            } 
            var shipper = await _unitOfWork.User.GetByCondition(u => u.UserId == confirmDeliveryDTO.ShipperId);
            var shipperProvince = await _unitOfWork.StorageProvince.GetByCondition(s => s.StorageProvinceId.Equals(shipper.StorageProvinceId));
            var order = await _unitOfWork.Order.GetByCondition(o => o.OrderId == orderStorage.OrderId);
            if (order.Status.Equals(OrderStatusConstant.ArrivalJapanAirport) || order.Status.Equals(OrderStatusConstant.ToShip) || order.Status.Equals(OrderStatusConstant.ArrivalJapanStorage))
            {   
                if (shipperProvince.Country.Equals(StorageCountryEnum.Vietnam))
                {
                    return new ResponseDTO("Storage in VN cannot confirm the order which is in JP", 400, false);
                }
            }
            if(order.Status.Equals(OrderStatusConstant.ToShip))
            {
                order.Status = OrderStatusConstant.ArrivalJapanStorage;
                _unitOfWork.Order.Update(order);
            } else
            if (order.Status.Equals(OrderStatusConstant.ArrivalJapanStorage))
            {
                order.Status = OrderStatusConstant.ArrivalJapanAirport;
                _unitOfWork.Order.Update(order);
                orderStorage.ArrivalTime = DateTime.Now;
                orderStorage.Status = false;
                _unitOfWork.OrderStorage.Update(orderStorage);
            }
            else
            if (order.Status.Equals(OrderStatusConstant.ArrivalJapanAirport))
            {
                order.Status = OrderStatusConstant.ArrivalVietnamStorage;
                _unitOfWork.Order.Update(order);
            }
            else
            if (order.Status.Equals(OrderStatusConstant.ArrivalVietnamStorage))
            {
                order.Status = OrderStatusConstant.ToReceive;
                _unitOfWork.Order.Update(order);
                orderStorage.ArrivalTime = DateTime.Now;
                orderStorage.Status = false;
                _unitOfWork.OrderStorage.Update(orderStorage);
            }
            else if (order.Status.Equals(OrderStatusConstant.ToReceive))
            {
                order.Status = OrderStatusConstant.Completed;
                _unitOfWork.Order.Update(order);
                orderStorage.ArrivalTime = DateTime.Now;
                orderStorage.Status = false;
                _unitOfWork.OrderStorage.Update(orderStorage);
            }
            var saveChanges = await _unitOfWork.SaveChangeAsync();  
            if(saveChanges)
            {
                return new ResponseDTO("Confirm completed mission sucessfully", 200, true);
            }
            return new ResponseDTO("Confirm completed mission fail", 500, false);
        }
    }
}
 