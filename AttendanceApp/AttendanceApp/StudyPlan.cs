using System;
using System.Collections.Generic;

namespace AttendanceApp;

public partial class StudyPlan
{
    public int IdStudyPlan { get; set; }

    public int Term { get; set; }

    public int IdSpec { get; set; }

    public int IdDiscipline { get; set; }

    public int LessonsCount { get; set; }

    public virtual Discipline IdDisciplineNavigation { get; set; } = null!;

    public virtual Specialization IdSpecNavigation { get; set; } = null!;
}
