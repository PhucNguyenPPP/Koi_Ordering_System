using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class User
{
    public Guid UserId { get; set; }

    public string UserName { get; set; } = null!;

    public byte[] Salt { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string AvatarLink { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public string Gender { get; set; } = null!;

    public int? OtpCode { get; set; }

    public DateTime? OtpExpiredTime { get; set; }

    public bool Status { get; set; }

    public Guid RoleId { get; set; }

    public Guid? StorageId { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Role Role { get; set; } = null!;

    public virtual Storage? Storage { get; set; }
}
