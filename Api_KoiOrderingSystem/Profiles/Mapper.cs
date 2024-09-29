using AutoMapper;
using Common.DTO.Auth;
using Common.DTO.Cart;
using Common.DTO.FarmImage;
using Common.DTO.KoiFish;
using Common.DTO.User;
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
            CreateMap<Koi, KoiDTO>().ReverseMap();
			CreateMap<User, FarmDetailDTO>().ReverseMap();
			CreateMap<FarmImage, FarmImageDTO>().ReverseMap();
			CreateMap<SignUpFarmRequestDTO, User>().ReverseMap();
            CreateMap<Cart, GetCartDTO>()
            .ForMember(dest => dest.KoiName, opt => opt.MapFrom(src => src.Koi.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Koi.Price));
            #endregion
        }
    }
}
