using System;
using System.Collections.Generic;

namespace AttendanceApp;

public partial class Schedule
{
    public int IdSchedule { get; set; }

    public int IdGroup { get; set; }

    public DateOnly LessonDate { get; set; }

    public int LessonNumber { get; set; }

    public int IdDiscipline { get; set; }

    public int IdTeacher { get; set; }

    public virtual Discipline IdDisciplineNavigation { get; set; } = null!;

    public virtual Group IdGroupNavigation { get; set; } = null!;

    public virtual Teacher IdTeacherNavigation { get; set; } = null!;

    public virtual ICollection<Truancy> Truancies { get; set; } = new List<Truancy>();
}
