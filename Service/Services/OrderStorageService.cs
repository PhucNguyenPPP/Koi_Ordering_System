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

        public async Task<ResponseDTO> AssignShipperJapan(AssignShipperDTO assignShipperDTO)
        {
            var update = false;
            var orderStorage = await CheckOrderStorageExist(assignShipperDTO.OrderId);
            if (orderStorage == null)
            {
                return new ResponseDTO("OrderStorage does not exist", 400, false);
            }
            var orderStorageJapan = GetOrderStorageJP(orderStorage);
            if (orderStorageJapan == null)
            {
                return new ResponseDTO("OrderStorage is not in Japan", 400, false);
            }          
            var checkOrderStatus = await CheckOrderStatusToAssignJP(assignShipperDTO.OrderId);
            if (!checkOrderStatus)
            {
                return new ResponseDTO("Cannot Assign order with this status", 400, false);
            }
            var storageProvinceId = orderStorageJapan.Select(o => o.StorageProvinceId).FirstOrDefault();
            var checkShipperProvince = await CheckShipperProvince(assignShipperDTO.ShipperId, storageProvinceId);
            if (!checkShipperProvince.IsSuccess)
            {
                return checkShipperProvince;
            }
            foreach ( var item in orderStorageJapan)
            {
                item.ShipperId = assignShipperDTO.ShipperId;
            }
            _unitOfWork.OrderStorage.UpdateRange(orderStorageJapan);
            var saveChanges = await _unitOfWork.SaveChangeAsync();
            if (saveChanges)
            {
                 return new ResponseDTO("Assign shipper sucessfully", 200, true);
            }
            return new ResponseDTO("Assign shipper failed", 500, true);
        }

        private  List<OrderStorage> GetOrderStorageJP(List<OrderStorage> orderStorage)
        {
            var storageProvinceJP =  _unitOfWork.StorageProvince.GetAllByCondition(sp => sp.Country.Equals(StorageCountryEnum.Japan.ToString())).ToList();
            var orderStorageJP = orderStorage.Where(os => storageProvinceJP.Any(sp => sp.StorageProvinceId.Equals(os.StorageProvinceId))).ToList();
            return orderStorageJP;
        }

        private async Task<ResponseDTO> CheckShipperProvince(Guid shipperId, Guid storageProvinceId)
        {
            var shipper = await _unitOfWork.User.GetByCondition(u => u.UserId == shipperId);
            if (shipper == null)
            {
                return new ResponseDTO("Shipper does not exist", 400, false);
            }
            if (!storageProvinceId.Equals(shipper.StorageProvinceId))
            {
                return new ResponseDTO("Storage Manager cannot assign this shipper", 400, false);
            }
            return new ResponseDTO("Check Shipper and Province successfully", 200, true);
        }

        private async Task<bool> CheckOrderStatusToAssignJP(Guid orderId)
        {
            var order = await _unitOfWork.Order.GetByCondition(o => o.OrderId.Equals(orderId));
            if (order.Status.Equals(OrderStatusConstant.ToShip))
            {
                return true;
            }
            return false;
        }
        private async Task<List<OrderStorage>>? CheckOrderStorageExist(Guid orderId)
        {
            var orderStorage =  _unitOfWork.OrderStorage.GetAllByCondition(o => o.OrderId == orderId).ToList();
            return orderStorage;
        }
/*
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
        */
        public async Task<IEnumerable<OrderShipperDTO>> GetOrdersForShipper(Guid shipperId)
        {
            return await _unitOfWork.OrderStorage.GetAssignedOrdersByShipper(shipperId);
        }
    }
}
 