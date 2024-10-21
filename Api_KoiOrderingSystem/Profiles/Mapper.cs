using AutoMapper;
using Common.DTO.Auth;
using Common.DTO.Cart;
using Common.DTO.FarmImage;
using Common.DTO.Flight;
using Common.DTO.KoiFarm;
using Common.DTO.KoiFish;
using Common.DTO.Order;
using Common.DTO.StorageProvince;
using DAL.Entities;

namespace Api_KoiOrderingSystem.Profiles
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            #region
            CreateMap<SignUpCustomerRequestDTO, User>().ReverseMap();
            CreateMap<User, LocalUserDTO>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName))
                .ForMember(dest => dest.FarmId, opt => opt.MapFrom(src => src.KoiFarm.KoiFarmId.ToString()))
                .ReverseMap();
            CreateMap<Koi, KoiDTO>()
                .ReverseMap();
			CreateMap<KoiFarm, FarmDetailDTO>().ReverseMap();
			CreateMap<SignUpFarmRequestDTO, User>().ReverseMap();
			CreateMap<SignUpFarmRequestDTO, KoiFarm>().ReverseMap();
			CreateMap<SignUpShipperRequestDTO, User>().ReverseMap();
            CreateMap<Koi, GetAllKoiDTO>()
                .ForMember(dest => dest.BreedId, opt => opt.MapFrom(src=> src.KoiBreeds.Select(c=> c.BreedId).ToList()))
                .ForMember(dest => dest.BreedName, opt => opt.MapFrom(src => src.KoiBreeds.Select(c=> c.Breed.Name).ToList()))
                .ForMember(dest => dest.FarmName, opt => opt.MapFrom(src => src.Farm.FarmName))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => CalculateAge(src.Dob)))
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Order.OrderId.ToString()))
                .ReverseMap();

            CreateMap<SignUpShipperRequestDTO, User>().ReverseMap();
			
            CreateMap<Cart, GetCartDTO>()
            .ForMember(dest => dest.KoiName, opt => opt.MapFrom(src => src.Koi.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Koi.Price))
            .ForMember(dest => dest.KoiAvatar, opt => opt.MapFrom(src => src.Koi.AvatarLink))
            .ForMember(dest => dest.FarmName, opt => opt.MapFrom(src => src.Koi.Farm.FarmName))
            .ForMember(dest => dest.FarmId, opt => opt.MapFrom(src => src.Koi.Farm.KoiFarmId))
            .ForMember(dest => dest.StorageProvinceJapanId, opt => opt.MapFrom(src => src.Koi.Farm.StorageProvinceId))
            .ReverseMap();
            CreateMap<Koi, KoiDetailDTO>()
                .ForMember(dest => dest.BreedName, opt => opt.MapFrom(src => src.KoiBreeds.Select(c => c.Breed.Name).ToList()))
                .ForMember(dest => dest.FarmName, opt => opt.MapFrom(src => src.Farm.FarmName))
                .ForMember(dest => dest.FarmAvatar, opt => opt.MapFrom(src => src.Farm.FarmAvatar))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => CalculateAge(src.Dob)))
                .ReverseMap();
            CreateMap<CreateOrderDTO, Order>()
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId));
            CreateMap<StorageProvince, ProvinceResponseDTO>().ReverseMap();
            
            CreateMap<PolicyDTO, Policy>()
                .ForMember(dest => dest.PolicyId, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Policy, PolicyDTO>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateOrderPackagingRequest, Order>()
                .ForMember(dest => dest.OrderId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => OrderStatusConstant.ToShip))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Order, GetAllHistoryOrderDTO>()
                .ForMember(dest => dest.FarmId, opt => opt.MapFrom(src => src.Kois.FirstOrDefault().FarmId))
                .ForMember(dest => dest.FarmName, opt => opt.MapFrom(src => src.Kois.FirstOrDefault().Farm.FarmName))
                .ReverseMap();

            CreateMap<Koi, KoiOrderDTO>()
                .ReverseMap();
            CreateMap<Order, GetAllFarmHistoryOrderDTO>()
                .ForMember(dest => dest.FarmId, opt => opt.MapFrom(src => src.Kois.FirstOrDefault().FarmId))
                .ForMember(dest => dest.FarmName, opt => opt.MapFrom(src => src.Kois.FirstOrDefault().Farm.FarmName))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FullName))
                .ReverseMap();
            CreateMap<Flight, GetAllFlightDTO>()
            .ForMember(dest => dest.DepartureAirportName, opt => opt.MapFrom(src => src.DepartureAirport.AirportName))
            .ForMember(dest => dest.ArrivalAirportName, opt => opt.MapFrom(src => src.ArrivalAirport.AirportName));
            #endregion
        }

        private int CalculateAge(DateTime dob)
        {
            var today = DateTime.Today;
            var age = today.Year - dob.Year;
            if (dob.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
