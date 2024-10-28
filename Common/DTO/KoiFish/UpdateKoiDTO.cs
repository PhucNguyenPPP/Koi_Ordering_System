using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Common.DTO.KoiFish
{
    public class UpdateKoiDTO
    {
        [Required(ErrorMessage = "Please input koi id")]
        public Guid KoiId { get; set; }

        [Required(ErrorMessage = "Please input koi name")]
        public string Name { get; set; } = null!;

        public IFormFile? AvatarLink { get; set; } = null!;

        public IFormFile? CertificationLink { get; set; } = null!;

        [Required(ErrorMessage = "Please input price of koi")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid price")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please input description of koi")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Please input DOB of koi")]
        public DateTime Dob { get; set; }

        [Required(ErrorMessage = "Please input gender of koi")]
        public string Gender { get; set; } = null!;

        [Required(ErrorMessage = "Please input weight of koi")]
        public double Weight { get; set; }

        [Required(ErrorMessage = "Please input koi's breed")]
        public List<Guid> BreedId { get; set; }

    }
}
