using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities;

public partial class User
{
    [Key]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string UserName { get; set; } = null!;

    [Required]
    public byte[] Salt { get; set; } = null!;

    [Required]
    public byte[] PasswordHash { get; set; } = null!;

    [MaxLength(100)]
    public string FullName { get; set; } = null!;

    [MaxLength(200)]
    public string AvatarLink { get; set; } = null!;

    [MaxLength(20)]
    public string Phone { get; set; } = null!;

    [MaxLength(200)]
    public string Address { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    [Required]
    [MaxLength(10)]
    public string Gender { get; set; } = null!;

    public int? OtpCode { get; set; }

    public DateTime? OtpExpiredTime { get; set; }

    public bool Status { get; set; }

    [ForeignKey("Role")]
    public Guid RoleId { get; set; }

    [ForeignKey("StorageProvince")]
    public Guid? StorageProvinceId { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual KoiFarm? KoiFarm { get; set; }

    public virtual ICollection<OrderStorage> OrderStorages { get; set; } = new List<OrderStorage>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual Role Role { get; set; } = null!;

    public virtual StorageProvince? StorageProvince { get; set; }
}
