using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.FarmImage
{
	public class FarmImageDTO
	{
		public Guid FarmImageId { get; set; }

		public string Link { get; set; } = null!;
	}
}
