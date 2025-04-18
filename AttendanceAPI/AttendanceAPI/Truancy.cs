using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AttendanceAPI;

public partial class Truancy
{
    [JsonIgnore]
    public int IdTruancy { get; set; }

    public int IdStudent { get; set; }

    public int IdReason { get; set; }

    [JsonIgnore]
    public int IdSchedule { get; set; }

    [JsonIgnore]
    public virtual TruancyReason IdReasonNavigation { get; set; } = null!;

    public virtual Schedule IdScheduleNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual Student IdStudentNavigation { get; set; } = null!;
}
