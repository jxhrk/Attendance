using System;
using System.Collections.Generic;

namespace AttendanceApp;

public partial class Teacher
{
    public int IdTeacher { get; set; }

    public int IdPerson { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual Person IdPersonNavigation { get; set; } = null!;

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
