using System;
using System.Collections.Generic;

namespace Hotebedscache.Domain.Entities;

public partial class User
{
    public int Id { get; set; }

    public string? Email { get; set; }

    public bool EmailVerified { get; set; }

    public string? Mobile { get; set; }

    public bool MobileVerified { get; set; }

    public string? Password { get; set; }

    public string? Role { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Updated { get; set; }
}
