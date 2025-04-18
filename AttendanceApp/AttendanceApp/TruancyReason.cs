using System;
using System.Collections.Generic;

namespace AttendanceApp;

public partial class TruancyReason
{
    public int IdReason { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Truancy> Truancies { get; set; } = new List<Truancy>();
}
