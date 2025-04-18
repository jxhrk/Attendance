using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AttendanceAPI;

public partial class Person
{
    public int IdPerson { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? MiddleName { get; set; }

    [JsonIgnore]
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    [JsonIgnore]
    public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();

    [JsonIgnore]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
