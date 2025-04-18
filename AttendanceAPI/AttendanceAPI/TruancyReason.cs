using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AttendanceAPI;

public partial class TruancyReason
{
    public int IdReason { get; set; }

    public string Name { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Truancy> Truancies { get; set; } = new List<Truancy>();
}
