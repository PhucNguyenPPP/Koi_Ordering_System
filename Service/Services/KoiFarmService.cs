using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Common.DTO.General;
using Common.DTO.KoiFarm;
using DAL.Entities;
using DAL.Interfaces;
using DAL.UnitOfWork;
using Service.Interfaces;

namespace Service.Services
{
	public class KoiFarmService : IKoiFarmService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public KoiFarmService(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageService)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public bool CheckFarmExist(string? farmName)
		{
			var farmList = _unitOfWork.KoiFarm.GetAll();
			if (farmList.Any(c => c.FarmName == farmName))
			{
				return true;
			}
			return false;
		}

		public async Task<ResponseDTO> GetFarmDetail(Guid farmId)
		{
			var farm = await _unitOfWork.KoiFarm.GetByCondition(u => u.KoiFarmId == farmId);
			if (farm != null)
			{
				FarmDetailDTO farmDetailDTO = _mapper.Map<FarmDetailDTO>(farm);
				return new ResponseDTO("Get farm information successfully", 200, true, farmDetailDTO);
			}
			return new ResponseDTO("Cannot find the farm", 404, false);
		}

	}
}
