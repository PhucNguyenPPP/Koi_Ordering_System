using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.DTO.FarmImage;


namespace Common.DTO.KoiFarm
{
	public class FarmDetailDTO
	{
		public string? FarmName { get; set; }

		public string? FarmDescription { get; set; }

		public string? FarmAddress { get; set; }
		public string FarmAvatar { get; set; } = null!;	}
}
