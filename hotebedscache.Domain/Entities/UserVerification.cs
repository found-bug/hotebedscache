using System;
using System.Collections.Generic;

namespace Hotebedscache.Domain.Entities;

public partial class UserVerification
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string Type { get; set; } = null!;

    public string Code { get; set; } = null!;

    public bool IsVerified { get; set; }

    public DateTime Created { get; set; }
}
