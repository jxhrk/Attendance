using System;
using System.Collections.Generic;

namespace AttendanceApp;

public partial class Specialization
{
    public int IdSpec { get; set; }

    public string NameId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<StudyPlan> StudyPlans { get; set; } = new List<StudyPlan>();
}
