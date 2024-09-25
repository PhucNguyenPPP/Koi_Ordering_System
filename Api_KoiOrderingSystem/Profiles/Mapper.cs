using AutoMapper;
using Common.DTO.Auth;
using DAL.Entities;

namespace Api_KoiOrderingSystem.Profiles
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            #region
            CreateMap<SignUpCustomerRequestDTO, User>().ReverseMap();
            #endregion
        }
    }
}
