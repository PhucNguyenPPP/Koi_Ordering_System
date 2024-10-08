using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.StorageProvince
{
    public class ProvinceResponseDTO
    {
        public Guid StorageProvinceId { get; set; }

        public string StorageName { get; set; } = null!;

        public string ProvinceName { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string Country { get; set; } = null!;

        public bool Status { get; set; }

        public Guid AirportId { get; set; }
    }
}
