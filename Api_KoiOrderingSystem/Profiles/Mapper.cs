using AutoMapper;
using Common.DTO.Auth;
using Common.DTO.Cart;
using Common.DTO.FarmImage;
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
                .ReverseMap();

            CreateMap<SignUpShipperRequestDTO, User>().ReverseMap();
			
            CreateMap<Cart, GetCartDTO>()
            .ForMember(dest => dest.KoiName, opt => opt.MapFrom(src => src.Koi.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Koi.Price))
            .ForMember(dest => dest.KoiAvatar, opt => opt.MapFrom(src => src.Koi.AvatarLink))
            .ForMember(dest => dest.FarmName, opt => opt.MapFrom(src => src.Koi.Farm.FarmName))
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
