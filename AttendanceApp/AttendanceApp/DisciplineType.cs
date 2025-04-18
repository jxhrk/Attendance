using System;
using System.Collections.Generic;

namespace AttendanceApp;

public partial class DisciplineType
{
    public int IdDisciplineType { get; set; }

    public string? NameId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Discipline> Disciplines { get; set; } = new List<Discipline>();
}
