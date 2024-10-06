using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Interfaces;
using DAL.UnitOfWork;
using Service.Interfaces;

namespace Service.Services
{
	public class KoiFarmService : IKoiFarmService
	{
		private readonly IUnitOfWork _unitOfWork;

		public KoiFarmService(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageService)
		{
			_unitOfWork = unitOfWork;
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
	}
}
