using Common.DTO.RefundImage;
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

        public Guid FarmProvinceId { get; set; }

        public Guid CustomerProvinceId { get; set; }

        public string CustomerProvince { get; set; }

        public Guid CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public Guid? FlightId { get; set; }

        public string? FlightCode { get; set; }

        public string? Airline { get; set; }

        public string? DepartureDate { get; set; }

        public string? ArrivalDate { get; set; }

        public string? DepartureAirport { get; set; }

        public string? ArrivalAirport { get; set; }

        public string? RefundDescription { get; set; }

        public string? RefundResponse { get; set; }

        public DateTime? RefundCreatedDate { get; set; }

        public DateTime? RefundConfirmedDate { get; set; }

        public DateTime? RefundCompletedDate { get; set; }

        public int? RefundPercentage { get; set; }

        public string? BankAccount { get; set; }

        public Guid? RefundPolicyId { get; set; }

        public PolicyDTO? RefundPolicy { get; set; }
        public List<RefundImageDTO>? RefundRequestMedia { get; set; }
    }
}
