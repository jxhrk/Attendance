using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AttendanceAPI;

public partial class DisciplineType
{
    public int IdDisciplineType { get; set; }

    public string? NameId { get; set; }

    public string Name { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Discipline> Disciplines { get; set; } = new List<Discipline>();
}
