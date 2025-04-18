using System;
using System.Collections.Generic;

namespace AttendanceApp;

public partial class Truancy
{
    public int IdTruancy { get; set; }

    public int IdStudent { get; set; }

    public int IdReason { get; set; }

    public int IdSchedule { get; set; }

    public virtual TruancyReason IdReasonNavigation { get; set; } = null!;

    public virtual Schedule IdScheduleNavigation { get; set; } = null!;

    public virtual Student IdStudentNavigation { get; set; } = null!;
}
