using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.KoiFish
{
    public class KoiDetailDTO
    {
        public Guid KoiId { get; set; }

        public string Name { get; set; } = null!;

        public string AvatarLink { get; set; } = null!;

        public string CertificationLink { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Age { get; set; }

        public double Weight { get; set; }

        public string Gender { get; set; } = null!;

        public decimal Price { get; set; }

        public List<string> BreedName { get; set; }

        public Guid FarmId { get; set; }

        public string FarmName { get; set; } = null!;

        public bool Status { get; set; }
        public string FarmAvatar { get; set; } = null!;
    }
}
