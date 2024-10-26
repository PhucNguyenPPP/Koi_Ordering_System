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
        private readonly IUserService _userService;
        public OrderStorageService(IMapper mapper, IUnitOfWork unitOfWork, IUserService userService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<ResponseDTO> AssignShipperJapan(AssignShipperDTO assignShipperDTO)
        {
  
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
        private List<OrderStorage> GetOrderStorageVN(List<OrderStorage> orderStorage)
        {
            var storageProvinceVN = _unitOfWork.StorageProvince.GetAllByCondition(sp => sp.Country.Equals(StorageCountryEnum.Vietnam.ToString())).ToList();
            var orderStorageVN = orderStorage.Where(os => storageProvinceVN.Any(sp => sp.StorageProvinceId.Equals(os.StorageProvinceId))).ToList();
            return orderStorageVN;
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
        private async Task<bool> CheckOrderStatusToAssignVN(Guid orderId)
        {
            var order = await _unitOfWork.Order.GetByCondition(o => o.OrderId.Equals(orderId));
            if (order.Status.Equals(OrderStatusConstant.ToShip)||order.Status.Equals(OrderStatusConstant.ArrivalJapanStorage)|| order.Status.Equals(OrderStatusConstant.ArrivalJapanAirport))
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

        public async Task<ResponseDTO> ConfirmDelivery(ConfirmDeliveryDTO confirmDeliveryDTO)
        {
            var orderStorage = await CheckOrderStorageExist(confirmDeliveryDTO.OrderId);
            if (orderStorage == null)
            {
                return new ResponseDTO("OrderStorage does not exist", 400, false);
            }
            var orderStorageOfShipper = orderStorage.Where(os => os.ShipperId.Equals(confirmDeliveryDTO.ShipperId) && os.Status).FirstOrDefault();
            if(orderStorageOfShipper == null)
            {
                return new ResponseDTO("Shipper cannot confirm this orderStorage", 400, false);
            }                  
            var shipperRole = await _userService.GetShipperRole();
            var shipper = await _unitOfWork.User.GetByCondition(u => u.UserId == confirmDeliveryDTO.ShipperId||u.RoleId == shipperRole.RoleId);
            if(shipper == null)
            {
                return new ResponseDTO("Shipper does not exist", 400, false);
            }              
            var order = await _unitOfWork.Order.GetByCondition(o => o.OrderId == confirmDeliveryDTO.OrderId);
            if(order.Status.Equals(OrderStatusConstant.ToShip))
            {
                order.Status = OrderStatusConstant.ArrivalJapanStorage;
                var orderStorageOfShipperFuture = orderStorage.Where(os => os.ShipperId.Equals(confirmDeliveryDTO.ShipperId) && !os.Status && os.ArrivalTime==null).FirstOrDefault();
                if (orderStorageOfShipperFuture == null)
                {
                    return new ResponseDTO("Next OrderStorage does not exist", 400, false);
                }
                orderStorageOfShipper.Status = false;
                orderStorageOfShipperFuture.Status = true;
                orderStorageOfShipper.ArrivalTime = DateTime.Now;
                _unitOfWork.OrderStorage.Update(orderStorageOfShipperFuture);
                _unitOfWork.Order.Update(order);
            } else
            if (order.Status.Equals(OrderStatusConstant.ArrivalJapanStorage))
            {
                order.Status = OrderStatusConstant.ArrivalJapanAirport;
                orderStorageOfShipper.ArrivalTime = DateTime.Now;
                var orderStorageOfShipperFuture = orderStorage.Where(os => !os.ShipperId.Equals(confirmDeliveryDTO.ShipperId) && !os.Status && os.ArrivalTime == null).FirstOrDefault();             
                if (orderStorageOfShipperFuture == null)
                {
                    return new ResponseDTO("Next OrderStorage does not exist", 400, false);
                }
                orderStorageOfShipper.Status = false;
                orderStorageOfShipperFuture.Status = true;
                _unitOfWork.OrderStorage.Update(orderStorageOfShipper);
                _unitOfWork.Order.Update(order);
            }
            else
            if (order.Status.Equals(OrderStatusConstant.ArrivalJapanAirport))
            {
                order.Status = OrderStatusConstant.ArrivalVietNamAirport;
                orderStorageOfShipper.ArrivalTime = DateTime.Now;
                var orderStorageOfShipperFuture = orderStorage.Where(os => os.ShipperId.Equals(confirmDeliveryDTO.ShipperId) && !os.Status && os.ArrivalTime == null).FirstOrDefault();
                if (orderStorageOfShipperFuture == null)
                {
                    return new ResponseDTO("Next OrderStorage does not exist", 400, false);
                }
                orderStorageOfShipper.Status = false;
                orderStorageOfShipperFuture.Status = true;
                _unitOfWork.OrderStorage.Update(orderStorageOfShipper);
                _unitOfWork.Order.Update(order);
            }
            else
            if (order.Status.Equals(OrderStatusConstant.ArrivalVietNamAirport))
            {
                order.Status = OrderStatusConstant.ArrivalVietnamStorage;
                orderStorageOfShipper.ArrivalTime = DateTime.Now;
                var orderStorageOfShipperFuture = orderStorage.Where(os => os.ShipperId.Equals(confirmDeliveryDTO.ShipperId) && !os.Status && os.ArrivalTime == null).FirstOrDefault();
                if (orderStorageOfShipperFuture == null)
                {
                    return new ResponseDTO("Next OrderStorage does not exist", 400, false);
                }
                orderStorageOfShipper.Status = false;
                orderStorageOfShipperFuture.Status = true;
                _unitOfWork.OrderStorage.Update(orderStorageOfShipper);
                _unitOfWork.Order.Update(order);
            }
            else if (order.Status.Equals(OrderStatusConstant.ArrivalVietnamStorage))
            {
                order.Status = OrderStatusConstant.ToReceive;               
                orderStorageOfShipper.ArrivalTime = DateTime.Now;
                orderStorageOfShipper.Status = false;
                _unitOfWork.OrderStorage.Update(orderStorageOfShipper);
                _unitOfWork.Order.Update(order);
            }
            var saveChanges = await _unitOfWork.SaveChangeAsync();  
            if(saveChanges)
            {
                return new ResponseDTO("Confirm completed mission sucessfully", 200, true);
            }
            return new ResponseDTO("Confirm completed mission fail", 500, false);
        }
        public async Task<IEnumerable<OrderShipperDTO>> GetOrdersForShipper(Guid shipperId)
        {
            return await _unitOfWork.OrderStorage.GetAssignedOrdersByShipper(shipperId);
        }

        public async Task<ResponseDTO> AssignShipperVietnam(AssignShipperDTO assignShipperDTO)
        {
            var orderStorage = await CheckOrderStorageExist(assignShipperDTO.OrderId);
            if (orderStorage == null)
            {
                return new ResponseDTO("OrderStorage does not exist", 400, false);
            }
            var orderStorageVietnam = GetOrderStorageVN(orderStorage);
            if (orderStorageVietnam == null)
            {
                return new ResponseDTO("OrderStorage is not in Vietnam", 400, false);
            }
            var checkOrderStatus = await CheckOrderStatusToAssignVN(assignShipperDTO.OrderId);
            if (!checkOrderStatus)
            {
                return new ResponseDTO("Cannot Assign order with this status", 400, false);
            }
            var storageProvinceId = orderStorageVietnam.Select(o => o.StorageProvinceId).FirstOrDefault();
            var checkShipperProvince = await CheckShipperProvince(assignShipperDTO.ShipperId, storageProvinceId);
            if (!checkShipperProvince.IsSuccess)
            {
                return checkShipperProvince;
            }
            foreach (var item in orderStorageVietnam)
            {
                item.ShipperId = assignShipperDTO.ShipperId;
            }
            _unitOfWork.OrderStorage.UpdateRange(orderStorageVietnam);
            var saveChanges = await _unitOfWork.SaveChangeAsync();
            if (saveChanges)
            {
                return new ResponseDTO("Assign shipper sucessfully", 200, true);
            }
            return new ResponseDTO("Assign shipper failed", 500, true);
        }
    }
}
 