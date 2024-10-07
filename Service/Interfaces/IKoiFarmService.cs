using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.DTO.General;

namespace DAL.Interfaces
{
	public interface IKoiFarmService
	{
		bool CheckFarmExist(string? farmName);
		Task<ResponseDTO> GetFarmDetail(Guid farmId);
	}
}
