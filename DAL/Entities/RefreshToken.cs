using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class RefreshToken
{
    public Guid RefreshTokenId { get; set; }

    public string JwtId { get; set; } = null!;

    public string RefreshToken1 { get; set; } = null!;

    public DateTime ExpiredAt { get; set; }

    public bool IsValid { get; set; }

    public Guid UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
