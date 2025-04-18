using System;
using System.Collections.Generic;

namespace AttendanceApp;

public partial class Group
{
    public int IdGroup { get; set; }

    public string Name { get; set; } = null!;

    public int? IdTeacherCurator { get; set; }

    public int? IdElder { get; set; }

    public int IdSpec { get; set; }

    public int CourseNumber { get; set; }

    public virtual Student? IdElderNavigation { get; set; }

    public virtual Specialization IdSpecNavigation { get; set; } = null!;

    public virtual Teacher? IdTeacherCuratorNavigation { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
