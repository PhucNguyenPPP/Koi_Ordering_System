using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Auth
{
    public class SignUpCustomerRequestDTO
    {
            [Required(ErrorMessage = "Vui lòng nhập tên tài khoản")]
            [MinLength(5, ErrorMessage = "Tên tài khoản phải có ít nhất 5 ký tự")]
            public string UserName { get; set; } = null!;

            [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
            [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự")]
            [RegularExpression("^(?=.*[!@#$%^&*(),.?\":{}|<>]).+$",
                ErrorMessage = "Mật khẩu phải có ít nhất 1 ký tự đặc biệt")]
            public string Password { get; set; } = null!;

            [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
            [MinLength(8, ErrorMessage = "Họ và tên phải có ít nhất 8 ký tự")]
            [RegularExpression("^[\\p{L}]+([\\s\\p{L}]+)*$",
                ErrorMessage = "Họ và tên không hợp lệ")]
            public string FullName { get; set; } = null!;

            [Required(ErrorMessage = "Vui lòng chọn ảnh đại diện")]
            public IFormFile AvatarLink { get; set; } = null!;

            [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
            [RegularExpression("^0\\d{9}$",
                ErrorMessage = "Số điện thoại không hợp lệ")]
            public string Phone { get; set; } = null!;

            [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
            public string Address { get; set; } = null!;

            [Required(ErrorMessage = "Vui lòng nhập email")]
            [EmailAddress]
            public string Email { get; set; } = null!;

            [Required(ErrorMessage = "Vui lòng nhập ngày sinh")]
            public DateTime DateOfBirth { get; set; }

            [Required(ErrorMessage = "Vui lòng chọn giới tính")]
            public string Gender { get; set; } = null!;
    }
}
