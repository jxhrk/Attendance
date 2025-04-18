using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AttendanceAPI;

public partial class Group
{
    public int IdGroup { get; set; }

    public string Name { get; set; } = null!;

    public int? IdTeacherCurator { get; set; }

    public int? IdElder { get; set; }

    public int IdSpec { get; set; }

    public int CourseNumber { get; set; }

    [JsonIgnore]
    public virtual Student? IdElderNavigation { get; set; }

    [JsonIgnore]
    public virtual Specialization IdSpecNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual Teacher? IdTeacherCuratorNavigation { get; set; }

    [JsonIgnore]
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    [JsonIgnore]
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
