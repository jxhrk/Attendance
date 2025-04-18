using System;
using System.Collections.Generic;

namespace AttendanceApp;

public partial class Discipline
{
    public int IdDiscipline { get; set; }

    public string? NameId { get; set; }

    public string Name { get; set; } = null!;

    public int? IdDisciplineType { get; set; }

    public virtual DisciplineType? IdDisciplineTypeNavigation { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<StudyPlan> StudyPlans { get; set; } = new List<StudyPlan>();
}
