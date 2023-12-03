using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class RefreshToken
{
    public Guid Id { get; set; }

    public int? UserId { get; set; }

    public string? Token { get; set; }

    public string? JwtId { get; set; }

    public bool? IsUsed { get; set; }

    public bool? IsRevoked { get; set; }

    public DateTime? IsUsedAt { get; set; }

    public DateTime? ExpiredAt { get; set; }

    public virtual User? User { get; set; }
}
