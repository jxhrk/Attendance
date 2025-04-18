using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AttendanceAPI;

public partial class StudyPlan
{
    public int IdStudyPlan { get; set; }

    public int Term { get; set; }

    public int IdSpec { get; set; }

    public int IdDiscipline { get; set; }

    public int LessonsCount { get; set; }

    [JsonIgnore]
    public virtual Discipline IdDisciplineNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual Specialization IdSpecNavigation { get; set; } = null!;
}
