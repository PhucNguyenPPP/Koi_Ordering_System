using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Common.DTO.General;
using Common.DTO.StorageProvince;
using Common.Enum;
using DAL.UnitOfWork;
using Service.Interfaces;

namespace Service.Services
{
	public class StorageProvinceService : IStorageProvinceService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		public StorageProvinceService(IUnitOfWork unitOfWork,
			 IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}
		public async Task<ResponseDTO> CheckJapanStorageProvince(Guid? storageProvinceId)
		{
			var list = _unitOfWork.StorageProvince.GetAll();
			if (list.Any(s => s.StorageProvinceId.Equals(storageProvinceId)))
			{
				if (list.Any(s => s.Country.Equals(StorageCountryEnum.Japan)))
				{
					return new ResponseDTO("Valid storage in Japan", 200, true);
				}
				else
				{
					return new ResponseDTO("The storage is not in Japan", 400, true);
				}
			}
			return new ResponseDTO("Not found the storage in Japan", 404, false); ;
		}

        public ResponseDTO GetStorageProvinceByContry(string? country)
        {
            if(country == StorageCountryEnum.Vietnam.ToString() 
				|| country == StorageCountryEnum.Japan.ToString())
			{
				var list = _unitOfWork.StorageProvince.GetAllByCondition(c => c.Country == country);
				var mapList = _mapper.Map<List<ProvinceResponseDTO>>(list);
				return new ResponseDTO("Get province successfully", 200, true, mapList);
			} else
			{
                var list = _unitOfWork.StorageProvince.GetAll();
                var mapList = _mapper.Map<List<ProvinceResponseDTO>>(list);
                return new ResponseDTO("Get province successfully", 200, true, mapList);
            }
        }
    }
}
