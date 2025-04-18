using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AttendanceAPI;

public partial class Specialization
{
    public int IdSpec { get; set; }

    public string NameId { get; set; } = null!;

    public string Name { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    [JsonIgnore]
    public virtual ICollection<StudyPlan> StudyPlans { get; set; } = new List<StudyPlan>();
}
