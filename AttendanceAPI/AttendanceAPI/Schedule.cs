using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AttendanceAPI;

public partial class Schedule
{
    [JsonIgnore]
    public int IdSchedule { get; set; }

    public int IdGroup { get; set; }

    public DateOnly LessonDate { get; set; }

    public int LessonNumber { get; set; }

    public int IdDiscipline { get; set; }

    public int IdTeacher { get; set; }

    [JsonIgnore]
    public virtual Discipline IdDisciplineNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual Group IdGroupNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual Teacher IdTeacherNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Truancy> Truancies { get; set; } = new List<Truancy>();
}
