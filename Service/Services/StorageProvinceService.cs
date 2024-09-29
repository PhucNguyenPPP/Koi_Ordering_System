using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.DTO.General;
using Common.Enum;
using DAL.UnitOfWork;
using Service.Interfaces;

namespace Service.Services
{
	public class StorageProvinceService : IStorageProvinceService
	{
		private readonly IUnitOfWork _unitOfWork;
		public StorageProvinceService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
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
	}
}
