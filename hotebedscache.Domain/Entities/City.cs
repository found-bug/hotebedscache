using System;
using System.Collections.Generic;

namespace Hotebedscache.Domain.Entities;

public partial class City
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
}
