using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.RefundImage
{
    public class RefundImageDTO
    {
        public Guid RefundRequestMediaId { get; set; }

        public string Link { get; set; } = null!;
    }
}
