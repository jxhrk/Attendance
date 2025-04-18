using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AttendanceAPI;

public partial class Student
{
    public int IdStudent { get; set; }

    public int IdPerson { get; set; }

    public int? IdGroup { get; set; }

    [JsonIgnore]
    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    [JsonIgnore]
    public virtual Group? IdGroupNavigation { get; set; }

    [JsonIgnore]
    public virtual Person IdPersonNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Truancy> Truancies { get; set; } = new List<Truancy>();
}
