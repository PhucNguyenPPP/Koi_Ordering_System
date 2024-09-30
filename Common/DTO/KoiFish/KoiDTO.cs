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
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn hình cá")]
        public IFormFile AvatarLink { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn giấy khai sinh cá")]
        public IFormFile CertificationLink { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập mô tả")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập ngày sinh")]
        public DateTime Dob { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giới tính")]
        public string Gender { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập giới tính")]
        [Range(1, int.MaxValue, ErrorMessage ="Giá không hợp lệ")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giống")]
        public Guid BreedId { get; set; }


        [Required(ErrorMessage = "Vui lòng nhập Farm")]
        public Guid FarmId { get; set; }
    }
}
