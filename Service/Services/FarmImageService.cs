using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Common.DTO.FarmImage;
using DAL.UnitOfWork;
using Service.Interfaces;

namespace Service.Services
{
	public class FarmImageService : IFarmImageService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		public FarmImageService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}
		public async Task<List<FarmImageDTO>> GetFarmImageByUserId(Guid userId)
		{
			var list = _unitOfWork.FarmImage.GetAllByCondition(f=>f.FarmId == userId).ToList();
			var listDTO = _mapper.Map<List<FarmImageDTO>>(list);
			return listDTO;
		}
	}
}
