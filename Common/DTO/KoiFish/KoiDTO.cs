using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Common.DTO.KoiFish
{
    public class KoiDTO
    {
        [Required(ErrorMessage = "Please input name of koi")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Please select image of koi")]
        public IFormFile? AvatarLink { get; set; } = null!;

        [Required(ErrorMessage = "Please select certification image of koi")]
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

        [Required(ErrorMessage = "Please input koi's farm")]
        public List<Guid> BreedId { get; set; }

        [Required(ErrorMessage = "Please input farm")]
        public Guid FarmId { get; set; }
    }
}
