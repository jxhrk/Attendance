using System;
using System.Collections.Generic;

namespace AttendanceApp;

public partial class Student
{
    public int IdStudent { get; set; }

    public int IdPerson { get; set; }

    public int? IdGroup { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual Group? IdGroupNavigation { get; set; }

    public virtual Person IdPersonNavigation { get; set; } = null!;

    public virtual ICollection<Truancy> Truancies { get; set; } = new List<Truancy>();
}
