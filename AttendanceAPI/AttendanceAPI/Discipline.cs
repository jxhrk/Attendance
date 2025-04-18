using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AttendanceAPI;

public partial class Discipline
{
    public int IdDiscipline { get; set; }

    public string? NameId { get; set; }

    public string Name { get; set; } = null!;

    public int? IdDisciplineType { get; set; }

    [JsonIgnore]
    public virtual DisciplineType? IdDisciplineTypeNavigation { get; set; }

    [JsonIgnore]
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    [JsonIgnore]
    public virtual ICollection<StudyPlan> StudyPlans { get; set; } = new List<StudyPlan>();
}
