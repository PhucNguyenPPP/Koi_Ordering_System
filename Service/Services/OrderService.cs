using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Common.DTO.General;
using Common.DTO.KoiFish;
using Common.DTO.Order;
using Common.Enum;
using DAL.Entities;
using DAL.UnitOfWork;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Service.Interfaces;

namespace Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFlightService _flightService;
        public OrderService(IMapper mapper, IUnitOfWork unitOfWork, IFlightService flightService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _flightService = flightService;
        }

        public async Task<ResponseDTO> CheckValidationCreateOrder(CreateOrderDTO createOrderDTO)
        {
            var customer = _unitOfWork.User.GetAllByCondition(c => c.UserId == createOrderDTO.CustomerId /*&& c.Role.RoleName.Equals(RoleEnum.Customer)*/);
            if (customer.IsNullOrEmpty())
                return new ResponseDTO("Customer not exist!", 400, false);

            Guid farm = Guid.NewGuid();
            for (int i = 0; i < createOrderDTO.CartId.Count; i++)
            {
                var cart = await _unitOfWork.Cart.GetByCondition(c => c.CartId == createOrderDTO.CartId[i]);
                if (cart == null)
                    return new ResponseDTO("Cart not exist!", 400, false);

                var koi = await _unitOfWork.Koi.GetByCondition(c => c.KoiId == cart.KoiId && c.Status == true);
                if (koi == null)
                    return new ResponseDTO("Koi not exist!", 400, false);
                if (i == 0) farm = koi.FarmId;

                if (i > 0)
                {
                    if (farm != koi.FarmId)
                    {
                        return new ResponseDTO("Unable to select Koi from different farms!", 400, false);
                    }
                }
            }


            var storage = _unitOfWork.StorageProvince.GetAllByCondition(c => c.Country.Equals("Vietnam"));
            if (!storage.Any(c => c.StorageProvinceId.Equals(createOrderDTO.StorageVietNamId)))
                return new ResponseDTO("Invalid Viet Nam storage province!", 400, false);

            return new ResponseDTO("Check validation successfully!", 200, true);
        }

        public async Task<ResponseDTO> CreateOrder(CreateOrderDTO createOrderDTO)
        {
            var order = _mapper.Map<Order>(createOrderDTO);
            if (createOrderDTO == null)
            {
                throw new ArgumentNullException(nameof(createOrderDTO), "createOrderDTO is null");
            }

            if (order == null)
            {
                throw new NullReferenceException("Mapping from CreateOrderDTO to Order failed.");
            }
            //OrderId
            var orderId = Guid.NewGuid();
            order.OrderId = orderId;

            //OrderNumber
            Random rand = new Random();
            string num = "";
            var orderNum = _unitOfWork.Order.GetAll();
            do
            {
                num = "B" + rand.Next(999);
                order.OrderNumber = num;
            } while (orderNum.Any(c => c.OrderNumber == num));

            order.StorageProvinceVietnamId = createOrderDTO.StorageVietNamId;
            //OrderCreateDate
            order.CreatedDate = DateTime.Now;

            //Order Status
            order.Status = OrderStatusConstant.Unpaid;
            List<Guid> koiId = new List<Guid>();
            decimal totalPrice = 0;
            decimal shippingFee = 0;
            Guid? jpnStorage = Guid.NewGuid();
            for (int i = 0; i < createOrderDTO.CartId.Count(); i++)
            {
                var cart = await _unitOfWork.Cart.GetByCondition(c => c.CartId == createOrderDTO.CartId[i]);
                koiId.Add(cart.KoiId);

                //update Koi
                var koi = _unitOfWork.Koi.GetAllByCondition(c => c.KoiId == koiId[i]).FirstOrDefault();
                koi.Status = false;
                koi.OrderId = orderId;
                _unitOfWork.Koi.Update(koi);

                totalPrice += cart.Koi.Price;
                if (i == 0)
                {
                    var farm = _unitOfWork.KoiFarm.GetAllByCondition(c => c.KoiFarmId == cart.Koi.FarmId).FirstOrDefault();

                    jpnStorage = farm.StorageProvinceId;
                    shippingFee = _unitOfWork.ShippingFee
                    .GetAllByCondition(c => c.StorageProvinceVnId == createOrderDTO.StorageVietNamId
                    && c.StorageProvinceJpId == jpnStorage)
                    .Select(c => c.Price).FirstOrDefault();
                    order.ShippingFee = shippingFee.ToString();
                }
                if (i == createOrderDTO.CartId.Count() - 1)
                {
                    totalPrice += shippingFee;
                    order.TotalPrice = totalPrice;


                    await _unitOfWork.Order.AddAsync(order);
                    await _unitOfWork.SaveChangeAsync();
                }
                _unitOfWork.Cart.Delete(cart);
            }
            //OrderStorage orderStorage1 = new OrderStorage();
            //orderStorage1.OrderId = orderId;
            //orderStorage1.Status = true;
            //if (jpnStorage != null) orderStorage1.StorageProvinceId = (Guid)jpnStorage;
            //orderStorage1.OrderStorageId = Guid.NewGuid();
            //await _unitOfWork.OrderStorage.AddAsync(orderStorage1);
            //await _unitOfWork.SaveChangeAsync();

            //OrderStorage orderStorage2 = new OrderStorage();
            //orderStorage2.OrderId = orderId;
            //orderStorage2.Status = false;
            //if (jpnStorage != null) orderStorage2.StorageProvinceId = (Guid)jpnStorage;
            //orderStorage2.OrderStorageId = Guid.NewGuid();
            //await _unitOfWork.OrderStorage.AddAsync(orderStorage2);
            //await _unitOfWork.SaveChangeAsync();

            //OrderStorage orderStorage3 = new OrderStorage();
            //orderStorage2.OrderId = orderId;
            //orderStorage2.Status = false;
            //orderStorage2.StorageProvinceId = createOrderDTO.StorageVietNamId;
            //orderStorage2.OrderStorageId = Guid.NewGuid();
            //await _unitOfWork.OrderStorage.AddAsync(orderStorage3);
            //await _unitOfWork.SaveChangeAsync();

            //OrderStorage orderStorage4 = new OrderStorage();
            //orderStorage2.OrderId = orderId;
            //orderStorage2.Status = false;
            //orderStorage2.StorageProvinceId = createOrderDTO.StorageVietNamId;
            //orderStorage2.OrderStorageId = Guid.NewGuid();
            //await _unitOfWork.OrderStorage.AddAsync(orderStorage4);

            OrderStorage orderStorage1 = new OrderStorage
            {
                OrderId = orderId,
                Status = true,
                StorageProvinceId = jpnStorage != null ? (Guid)jpnStorage : Guid.Empty, // Ensure a valid ID
                OrderStorageId = Guid.NewGuid()
            };
            await _unitOfWork.OrderStorage.AddAsync(orderStorage1);
            await _unitOfWork.SaveChangeAsync();

            // Create and add the second OrderStorage
            OrderStorage orderStorage2 = new OrderStorage
            {
                OrderId = orderId,
                Status = false,
                StorageProvinceId = jpnStorage != null ? (Guid)jpnStorage : Guid.Empty,
                OrderStorageId = Guid.NewGuid()
            };
            await _unitOfWork.OrderStorage.AddAsync(orderStorage2);
            await _unitOfWork.SaveChangeAsync();

            // Create and add the third OrderStorage
            OrderStorage orderStorage3 = new OrderStorage
            {
                OrderId = orderId,
                Status = false,
                StorageProvinceId = createOrderDTO.StorageVietNamId,
                OrderStorageId = Guid.NewGuid()
            };
            await _unitOfWork.OrderStorage.AddAsync(orderStorage3);
            await _unitOfWork.SaveChangeAsync();

            // Create and add the fourth OrderStorage
            OrderStorage orderStorage4 = new OrderStorage
            {
                OrderId = orderId,
                Status = false,
                StorageProvinceId = createOrderDTO.StorageVietNamId,
                OrderStorageId = Guid.NewGuid()
            };
            await _unitOfWork.OrderStorage.AddAsync(orderStorage4);
            await _unitOfWork.SaveChangeAsync();
            OrderStorage orderStorage5 = new OrderStorage
            {
                OrderId = orderId,
                Status = false,
                StorageProvinceId = createOrderDTO.StorageVietNamId,
                OrderStorageId = Guid.NewGuid()
            };
            await _unitOfWork.OrderStorage.AddAsync(orderStorage5);


            var result = await _unitOfWork.SaveChangeAsync();
            if (result)
            {
                return new ResponseDTO("Create order successfully!", 201, true, orderId);
            }
            else
            {
                return new ResponseDTO("Create order failed!", 400, true, null);
            }
        }

        public async Task<bool> CheckOrderExist(Guid orderId)
        {
            var order = await _unitOfWork.Order.GetByCondition(c => c.OrderId == orderId);
            if (order == null)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> UpdateOrderPackaging(Guid orderId, UpdateOrderPackagingRequest request)
        {
            // Find order
            Order? order = await _unitOfWork.Order.GetByCondition(o => o.OrderId == orderId);
            if (order == null)
            {
                return false;
            }

            // Check status order is different processing
            if (order.Status != OrderStatusConstant.Processing)
            {
                return false;
            }

            // Map non-null properties from request to entity
            _mapper.Map(request, order);

            // Update order using UnitOfWork
            _unitOfWork.Order.Update(order);

            // Save changes
            bool saveResult = await _unitOfWork.SaveChangeAsync();

            return saveResult;
        }

        public async Task<bool> UpdateOrderCompleted(Guid orderId)
        {
            // Find order
            Order? order = await _unitOfWork.Order.GetByCondition(o => o.OrderId == orderId);
            if (order == null)
            {
                return false;
            }

            // Check status order is different processing
            if (order.Status != OrderStatusConstant.ToReceive)
            {
                return false;
            }

            order.Status = OrderStatusConstant.Completed;
            order.PackagedDate =  DateTime.Now;

            // Update order using UnitOfWork
            _unitOfWork.Order.Update(order);

            // Save changes
            bool saveResult = await _unitOfWork.SaveChangeAsync();

            return saveResult;
        }

        public async Task<ResponseDTO> GetAllHistoryOrder(Guid customerId)
        {
            var customer = _unitOfWork.User.GetAllByCondition(c => c.UserId == customerId && c.Role.RoleName == RoleEnum.Customer.ToString());
            if (customer.IsNullOrEmpty())
            {
                return new ResponseDTO("Invalid customer!", 400, false);
            }
            
                var order = _unitOfWork.Order
                .GetAllByCondition(c=> c.CustomerId == customerId)
                .Include(c=> c.Kois).ThenInclude(c=> c.Farm)
                .ToList();
            if (order == null || !order.Any())  return new ResponseDTO("Your history order list is empty!", 400, false);
            var list = _mapper.Map<List<GetAllHistoryOrderDTO>>(order);
            return new ResponseDTO("List displayed successfully", 200, true, list);
        }

        public async Task<ResponseDTO> GetAllFarmHistoryOrder(Guid farmId)
        {
            var farm = _unitOfWork.KoiFarm.GetAllByCondition(c => c.KoiFarmId == farmId);
            if (farm.IsNullOrEmpty())
            {
                return new ResponseDTO("Invalid koi farm!", 400, false);
            }
            var order = _unitOfWork.Order
                .GetAllByCondition(c=> c.Kois.FirstOrDefault().FarmId == farmId && c.Status != OrderStatusConstant.Unpaid)
                .Include(c=> c.Customer)
                .Include(c => c.Kois).ThenInclude(c => c.Farm)
                .ToList();
            if (order == null || !order.Any()) return new ResponseDTO("Your history order list is empty!", 400, false);
            var list = _mapper.Map<List<GetAllFarmHistoryOrderDTO>>(order);
            return new ResponseDTO("List displayed successfully", 200, true, list);
        }

        public async Task<ResponseDTO> GetAllStorageHistoryOrder(Guid storageProvinceId)
        {
            var storageProvince = _unitOfWork.StorageProvince.GetAllByCondition(c=> c.StorageProvinceId == storageProvinceId);
            if (storageProvince.IsNullOrEmpty())
            {
                return new ResponseDTO("Invalid storage province!", 400, false);
            }

            var order = _unitOfWork.Order
                .GetAllByCondition(c => (c.Kois.FirstOrDefault().Farm.StorageProvinceId == storageProvinceId || c.StorageProvinceVietnamId == storageProvinceId)
                && c.Status != OrderStatusConstant.Unpaid
                && c.Status != OrderStatusConstant.Processing)
                .Include(c => c.Customer)
                .Include(c => c.Kois).ThenInclude(c => c.Farm)
                .ToList();
            if (order == null || !order.Any()) return new ResponseDTO("Your history order list is empty!", 400, false);
            var list = _mapper.Map<List<GetAllFarmHistoryOrderDTO>>(order);
            return new ResponseDTO("List displayed successfully", 200, true, list);
        }

        public async Task<ResponseDTO> GetOrderDetail(Guid orderId)
        {
            var order = _unitOfWork.Order
            .GetAllByCondition(c => c.OrderId == orderId)
            .Include(c => c.Customer)
            .Include(c => c.OrderStorages)
            .ThenInclude(c => c.Shipper)
            .Include(c => c.OrderStorages)
            .ThenInclude(c => c.StorageProvince)
            .Include(c => c.Flight)
            .ThenInclude(c => c.DepartureAirport)
            .Include(c => c.Flight)
            .ThenInclude(c => c.ArrivalAirport)
            .Include(c => c.Kois)
            .ThenInclude(c => c.Farm)
            .ThenInclude(c => c.KoiFarmManager)
            .FirstOrDefault();

            if (order == null)
            {
                return new ResponseDTO("Invalid order id!", 400, false);
            }
            var mappedOrder = _mapper.Map<OrderDetailDTO>(order);
            var provinceName = order.OrderStorages
                .FirstOrDefault(c => c.StorageProvince.Country != StorageCountryEnum.Japan.ToString())
                .StorageProvince.ProvinceName;

            var japaneseOrderStorage = order.OrderStorages
                .FirstOrDefault(c => c.StorageProvince.Country == StorageCountryEnum.Japan.ToString());

            var vietnameseOrderStorage = order.OrderStorages
               .FirstOrDefault(c => c.StorageProvince.Country == StorageCountryEnum.Vietnam.ToString());

            mappedOrder.CustomerProvinceId = order.StorageProvinceVietnamId;
            mappedOrder.FarmProvinceId = japaneseOrderStorage.StorageProvinceId;
            mappedOrder.JapaneseShipper = japaneseOrderStorage.Shipper?.FullName;
            mappedOrder.VietnameseShipper = vietnameseOrderStorage.Shipper?.FullName;
            mappedOrder.CustomerProvince = provinceName;
            return new ResponseDTO("Get order detail successfully", 200, true, mappedOrder);
        }

        public async Task<ResponseDTO> AssignFlightToOrder(AssignFlightToOrderDTO assignFlightToOrderDTO)
        {
            var checkOrder = await CheckOrderExist(assignFlightToOrderDTO.OrderId);
            if (!checkOrder)
            {
                return new ResponseDTO("Order does not exist",400,false);
            }
            var order = await _unitOfWork.Order.GetByCondition(o => o.OrderId.Equals(assignFlightToOrderDTO.OrderId));
            if(order.Status!=OrderStatusConstant.Packaged)
            {
                return new ResponseDTO("Cannot assign the order with this status", 400, false);
            }
            var flight = await _unitOfWork.Flight.GetByCondition(f=>f.FlightId.Equals(assignFlightToOrderDTO.FlightId));    
            if(flight==null)
            {
                return new ResponseDTO("Flight does not exist", 400, false);
            }
            var storageVietnamId = order.StorageProvinceVietnamId;
            var airportVN = await _unitOfWork.StorageProvince.GetByCondition(s=> s.StorageProvinceId.Equals(storageVietnamId));
            var storageJapan = await _unitOfWork.OrderStorage.GetByCondition(os => !os.StorageProvinceId.Equals(storageVietnamId) && os.OrderId.Equals(assignFlightToOrderDTO.OrderId));
            var airporJP = await _unitOfWork.StorageProvince.GetByCondition(s => s.StorageProvinceId.Equals(storageJapan.StorageProvinceId));
            var finalFlight = await _unitOfWork.Flight.GetByCondition(f => f.FlightId.Equals(assignFlightToOrderDTO.FlightId) && f.DepartureAirportId.Equals(airporJP.AirportId) && f.ArrivalAirportId.Equals(airportVN.AirportId));
            if (finalFlight == null)
            {
                return new ResponseDTO("Flight are not suitable for Departure Province and Arrival Province", 400, false);
            }
            var departureDateTime = flight.DepartureDate;
            TimeSpan timeRemaining = departureDateTime - DateTime.Now;

            // Kiểm tra nếu thời gian còn lại nhỏ hơn 3 giờ
            if (timeRemaining.TotalHours < 3)
            {
                return new ResponseDTO("Please assign a flight that departs at least 3 hours from now", 400, false);
            }
            order.FlightId = assignFlightToOrderDTO.FlightId;
            _unitOfWork.Order.Update(order);
            var saveChanges = await _unitOfWork.SaveChangeAsync();    
            if(!saveChanges)
            {
                return new ResponseDTO("Assign flight to order failed", 500, false);
            }    
            return new ResponseDTO("Assign flight to order sucessfully", 200, true);
        }
    }
}
