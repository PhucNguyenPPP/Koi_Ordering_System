using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Order
{
    public class KoiOrderDTO
    {
        public Guid KoiId { get; set; }
        public string AvatarLink { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public double Weight { get; set; }
        public string Age { get; set; }
        public List<Guid> BreedId { get; set; }
        public List<string> BreedName { get; set; } = null!;
    }
}
