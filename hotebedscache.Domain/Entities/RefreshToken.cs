using System;
using System.Collections.Generic;

namespace Hotebedscache.Domain.Entities;

public partial class RefreshToken
{
    public long Id { get; set; }

    public string? Token { get; set; }

    public DateTime? Expires { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Revoked { get; set; }

    public string? ReplacedBy { get; set; }

    public int? UserId { get; set; }
}
