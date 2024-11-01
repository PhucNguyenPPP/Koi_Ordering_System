using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Common.DTO.General;
using Common.DTO.OrderStorage;
using Common.Enum;
using DAL.Entities;
using DAL.Interfaces;
using DAL.UnitOfWork;
using Microsoft.Identity.Client.Extensions.Msal;
using Microsoft.OpenApi.Writers;
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
                return new ResponseDTO("A shipper can only be assigned to an order after it has reached the 'packaged' status and a flight has been assigned.", 400, false);
            }
            var storageProvinceId = orderStorageJapan.Select(o => o.StorageProvinceId).FirstOrDefault();
            var checkShipperProvince = await CheckShipperProvince(assignShipperDTO.ShipperId, storageProvinceId);
            if (!checkShipperProvince.IsSuccess)
            {
                return checkShipperProvince;
            }
            foreach (var item in orderStorageJapan)
            {
                item.ShipperId = assignShipperDTO.ShipperId;
            }
            _unitOfWork.OrderStorage.UpdateRange(orderStorageJapan);
            var order = await _unitOfWork.Order.GetByCondition(o => o.OrderId == assignShipperDTO.OrderId);
            order.ToShipDate = DateTime.Now;
            order.Status = OrderStatusConstant.ToShip;
            _unitOfWork.Order.Update(order);
            var saveChanges = await _unitOfWork.SaveChangeAsync();
            if (saveChanges)
            {
                return new ResponseDTO("Assign shipper sucessfully", 200, true);
            }
            return new ResponseDTO("Assign shipper failed", 500, true);
        }

        private List<OrderStorage> GetOrderStorageJP(List<OrderStorage> orderStorage)
        {
            var storageProvinceJP = _unitOfWork.StorageProvince.GetAllByCondition(sp => sp.Country.Equals(StorageCountryEnum.Japan.ToString())).ToList();
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
            if (order.Status.Equals(OrderStatusConstant.Packaged) && order.FlightId != null)
            {
                return true;
            }
            return false;
        }
        private async Task<bool> CheckOrderStatusToAssignVN(Guid orderId)
        {
            var order = await _unitOfWork.Order.GetByCondition(o => o.OrderId.Equals(orderId));
            if (order.Status.Equals(OrderStatusConstant.ToShip) || order.Status.Equals(OrderStatusConstant.ArrivalJapanStorage) || order.Status.Equals(OrderStatusConstant.ArrivalJapanAirport))
            {
                return true;
            }
            return false;
        }
        private async Task<List<OrderStorage>>? CheckOrderStorageExist(Guid orderId)
        {
            var orderStorage = _unitOfWork.OrderStorage.GetAllByCondition(o => o.OrderId == orderId).ToList();
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
            if (orderStorageOfShipper == null)
            {
                return new ResponseDTO("Shipper cannot confirm this orderStorage", 400, false);
            }
            var shipperRole = await _userService.GetShipperRole();
            var shipper = await _unitOfWork.User.GetByCondition(u => u.UserId == confirmDeliveryDTO.ShipperId || u.RoleId == shipperRole.RoleId);
            if (shipper == null)
            {
                return new ResponseDTO("Shipper does not exist", 400, false);
            }
            var order = await _unitOfWork.Order.GetByCondition(o => o.OrderId == confirmDeliveryDTO.OrderId);
            if (order.Status.Equals(OrderStatusConstant.ToShip))
            {               
                var orderStorageOfShipperFuture = orderStorage.Where(os => os.ShipperId.Equals(confirmDeliveryDTO.ShipperId) && !os.Status && os.ArrivalTime == null).FirstOrDefault();
                if (orderStorageOfShipperFuture == null)
                {
                    return new ResponseDTO("Next OrderStorage does not exist", 400, false);
                }
                var departureDateTime = await _unitOfWork.Flight.GetByCondition(f => f.FlightId.Equals(order.FlightId));
                TimeSpan timeRemaining = departureDateTime.DepartureDate - DateTime.Now;

                // Kiểm tra nếu thời gian còn lại nhỏ hơn 2 giờ
                if (timeRemaining.TotalHours < 2)
                {
                    return new ResponseDTO("The shipment is late. Please contact the manager to resolve the delay.", 400, false); 
                    //nếu được khoá nút confirm lại
                }
                order.Status = OrderStatusConstant.ArrivalJapanStorage;
                orderStorageOfShipper.Status = false;
                orderStorageOfShipperFuture.Status = true;
                orderStorageOfShipper.ArrivalTime = DateTime.Now;
                _unitOfWork.OrderStorage.Update(orderStorageOfShipperFuture);
                _unitOfWork.Order.Update(order);
            }
            else
            if (order.Status.Equals(OrderStatusConstant.ArrivalJapanStorage))
            {
                var departureDateTime = await _unitOfWork.Flight.GetByCondition(f => f.FlightId.Equals(order.FlightId));
                var arrvial = DateTime.Now; 

                // Kiểm tra nếu thời gian bị trễ
                if (arrvial>departureDateTime.DepartureDate)
                {
                    return new ResponseDTO("The shipment is late. Please contact the manager to resolve the delay.", 400, false); 
                }
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
                var departureDateTime = await _unitOfWork.Flight.GetByCondition(f => f.FlightId.Equals(order.FlightId));
                var arrvial = DateTime.Now;

                // Kiểm tra nếu thời gian bị trễ
                if (arrvial < departureDateTime.DepartureDate)
                {
                    return new ResponseDTO("The flight will depart later", 400, false);
                }
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
            else
            {
                return new ResponseDTO("Cannot confirm the order with this status", 400, false);
            }
        
            var saveChanges = await _unitOfWork.SaveChangeAsync();
            if (saveChanges)
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

        public async Task<ResponseDTO> GetDeliveryOfOrder(Guid orderId)
        {
            var order = await _unitOfWork.Order.GetByCondition(o => o.OrderId.Equals(orderId));
            if(order == null)
            {
                return new ResponseDTO("Order does not exist", 400, false);
            }
            var flight = await _unitOfWork.Flight.GetByCondition(f => f.FlightId.Equals(order.FlightId));
            List<OrderStorage> orderstorage = _unitOfWork.OrderStorage.GetAllByCondition(os => os.OrderId.Equals(orderId) && !os.Status && os.ArrivalTime != null).OrderBy(os => os.ArrivalTime).ToList();
            var temp = orderstorage;
            var status = order.Status;
            List<DeliveryOfOrderDTO> deliveryOfOrderDTOs = new List<DeliveryOfOrderDTO>();
            DeliveryOfOrderDTO deliveryOfOrderDTO0 = new()
            {
                Status = "Order is created",
                ArrivalTime = order.CreatedDate,
            };
            deliveryOfOrderDTOs.Add(deliveryOfOrderDTO0);
            if(status!=OrderStatusConstant.Unpaid.ToString() && order.PackagedDate!=null && status != OrderStatusConstant.Processing.ToString())
            {
                DeliveryOfOrderDTO deliveryOfOrderDTO00 = new()
                {
                    Status = "Order is packaged",
                    ArrivalTime = order.PackagedDate,
                };
                deliveryOfOrderDTOs.Add(deliveryOfOrderDTO00);
                if (status != OrderStatusConstant.Packaged)
                {
                    DeliveryOfOrderDTO deliveryOfOrderDTO000 = new()
                    {
                        Status = "Order is ready to ship",
                        ArrivalTime = order.ToShipDate,
                    };
                    deliveryOfOrderDTOs.Add(deliveryOfOrderDTO000);

                }

            }
            if (temp.Count > 0)
            {    
                var storageJP = await _unitOfWork.StorageProvince.GetByCondition(sp => sp.StorageProvinceId.Equals(temp.First().StorageProvinceId));
                DeliveryOfOrderDTO deliveryOfOrderDTO1 = new()
                {
                    Status = "Arrive to "+storageJP.StorageName,
                    ArrivalTime = temp.First().ArrivalTime,

                };
                temp.Remove(temp.First());
                deliveryOfOrderDTOs.Add(deliveryOfOrderDTO1);
            }
            if (temp.Count > 0)
            {
                var airport = await _unitOfWork.Airport.GetByCondition(a => a.AirportId.Equals(flight.DepartureAirportId));
                DeliveryOfOrderDTO deliveryOfOrderDTO2 = new()
                {
                    Status = "Arrive to " + airport.AirportName,
                    ArrivalTime = temp.First().ArrivalTime,
                };
                temp.Remove(temp.First());
                deliveryOfOrderDTOs.Add(deliveryOfOrderDTO2);
            }
            if (temp.Count > 0)
            {
                var airport = await _unitOfWork.Airport.GetByCondition(a => a.AirportId.Equals(flight.ArrivalAirportId));
                DeliveryOfOrderDTO deliveryOfOrderDTO3 = new()
                {
                    Status = "Arrive to " + airport.AirportName,
                    ArrivalTime = temp.First().ArrivalTime,
                };
                temp.Remove(temp.First());
                deliveryOfOrderDTOs.Add(deliveryOfOrderDTO3);
            }
            if (temp.Count > 0)
            {
                var storageVN = await _unitOfWork.StorageProvince.GetByCondition(sp => sp.StorageProvinceId.Equals(temp.First().StorageProvinceId));
                DeliveryOfOrderDTO deliveryOfOrderDTO4 = new()
                {
                    Status = "Arrive to " + storageVN.StorageName,
                    ArrivalTime = temp.First().ArrivalTime,
                };
                temp.Remove(temp.First());
                deliveryOfOrderDTOs.Add(deliveryOfOrderDTO4);
            }
            if (temp.Count > 0)
            {
                DeliveryOfOrderDTO deliveryOfOrderDTO5 = new()
                {
                    Status = OrderStatusConstant.ToReceive,
                    ArrivalTime = temp.First().ArrivalTime,
                };
                temp.Remove(temp.First());
                deliveryOfOrderDTOs.Add(deliveryOfOrderDTO5);
            }
            if (order.RefundCreatedDate != null)
            {
                DeliveryOfOrderDTO deliveryOfOrderDT06 = new()
                {
                    Status = "Refund request is created",
                    ArrivalTime = order.RefundCreatedDate,
                };
                deliveryOfOrderDTOs.Add(deliveryOfOrderDT06);
            }
            if (order.RefundConfirmedDate != null)
            {
                DeliveryOfOrderDTO deliveryOfOrderDT07 = new()
                {
                    Status = "Refund request is confirmed",
                    ArrivalTime = order.RefundConfirmedDate,
                };
                deliveryOfOrderDTOs.Add(deliveryOfOrderDT07);
            }
            if (order.RefundCompletedDate != null)
            {
                DeliveryOfOrderDTO deliveryOfOrderDT08 = new()
                {
                    Status = "Refund request is completed",
                    ArrivalTime = order.RefundConfirmedDate,
                };
                deliveryOfOrderDTOs.Add(deliveryOfOrderDT08);
            }
            return new ResponseDTO("Get delivery of order sucessfully!", 200, true, deliveryOfOrderDTOs);
        }
    }
}
