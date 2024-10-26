using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Order
{
    public class OrderDetailDTO
    {
        public Guid OrderId { get; set; }
        public Guid FarmId { get; set; }
        public string FarmName { get; set; } = null!;
        public string OrderNumber { get; set; } = null!;
        public List<KoiOrderDTO> Kois { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Phone { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string ShippingFee { get; set; } = null!;

        public decimal TotalPrice { get; set; }

        public int? Rating { get; set; }

        public string? Feedback { get; set; }

        public int? Weight { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public int? Length { get; set; }

        public string JapaneseShipper { get; set; }

        public string VietnameseShipper { get; set; }

        public string Status { get; set; } = null!;

        public string FarmAddress { get; set; }

        public string FarmPhone { get; set; }

        public string CustomerProvince { get; set; }

        public Guid CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public Guid? FlightId { get; set; }

        public string? FlightName { get; set; }
    }
}
