using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.DTO.FarmImage;
using DAL.Entities;

namespace Service.Interfaces
{
	public interface IFarmImageService
	{
		Task<List<FarmImageDTO>> GetFarmImageByUserId(Guid userId);
	}
}
