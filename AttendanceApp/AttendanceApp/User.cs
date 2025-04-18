using System;
using System.Collections.Generic;

namespace AttendanceApp;

public partial class User
{
    public int IdUser { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int IdRole { get; set; }

    public int IdPerson { get; set; }

    public virtual Person IdPersonNavigation { get; set; } = null!;

    public virtual Role IdRoleNavigation { get; set; } = null!;
}
