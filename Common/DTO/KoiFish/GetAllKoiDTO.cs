﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.KoiFish
{
    public class GetAllKoiDTO
    {
        [Key]
        public Guid KoiId { get; set; }

        public string Name { get; set; } = null!;

        public string AvatarLink { get; set; } = null!;

        public string CertificationLink { get; set; } = null!;

        public string Description { get; set; } = null!;

        public DateTime Dob { get; set; }
        public string Age { get; set; }

        public string Gender { get; set; } = null!;

        public double Weight { get; set; }

        public decimal Price { get; set; } 

        public  List<Guid> BreedId { get; set; }

        public List<string> BreedName { get; set; } = null!;

        public Guid FarmId { get; set; }

        public string FarmName { get; set; } = null!;

        public bool Status { get; set; }
        public Guid? OrderId { get; set; }
    }
}
