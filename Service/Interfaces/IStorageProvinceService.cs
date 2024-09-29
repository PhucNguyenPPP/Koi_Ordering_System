using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.DTO.General;

namespace Service.Interfaces
{
	public interface IStorageProvinceService
	{
		Task<ResponseDTO> CheckJapanStorageProvince(Guid? storageProvinceId);
	}
}
