using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AttendanceAPI;

public partial class Teacher
{
    public int IdTeacher { get; set; }

    public int IdPerson { get; set; }

    [JsonIgnore]
    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    [JsonIgnore]
    public virtual Person IdPersonNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
