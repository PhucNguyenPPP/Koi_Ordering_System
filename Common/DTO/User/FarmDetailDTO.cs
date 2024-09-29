using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.DTO.FarmImage;


namespace Common.DTO.User
{
	public class FarmDetailDTO
	{
		public string? FarmName { get; set; }

		public string? FarmDescription { get; set; }

		public string? FarmAddress { get; set; }

		public bool Status { get; set; }
		public string AvatarLink { get; set; } = null!;
		public List<FarmImageDTO>? FarmImages { get; set; } 
	}
}
