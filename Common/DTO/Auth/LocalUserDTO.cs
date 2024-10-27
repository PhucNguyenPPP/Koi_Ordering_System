using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Auth
{
    public class LocalUserDTO
    {
        public Guid UserId { get; set; }

        public string UserName { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string AvatarLink { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string Email { get; set; } = null!;

        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; } = null!;
        public bool Status { get; set; }
        public string RoleName { get; set; } = null!;
        public string? FarmId { get; set; }
        public Guid? StorageProvinceId { get; set; }
        public string? Country { get; set; }
    }
}
