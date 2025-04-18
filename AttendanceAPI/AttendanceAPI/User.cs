using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AttendanceAPI;

public partial class User
{
    public int IdUser { get; set; }

    public string Login { get; set; } = null!;

    [JsonIgnore]
    public string Password { get; set; } = null!;

    public int IdRole { get; set; }

    public int IdPerson { get; set; }

    [JsonIgnore]
    public virtual Person IdPersonNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual Role IdRoleNavigation { get; set; } = null!;
}
