using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.KoiFish
{
    public class GetAllKoiDTO
    {
        Guid KoiId { get; set; }

        public string Name { get; set; } = null!;

        public string AvatarLink { get; set; } = null!;

        public string CertificationLink { get; set; } = null!;

        public string Description { get; set; } = null!;

        public DateTime Dob { get; set; }

        public string Gender { get; set; } = null!;

        public bool Status { get; set; }
    }
}
