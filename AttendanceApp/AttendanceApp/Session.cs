using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceApp
{
    public class Session
    {
        public User user { get; set; }
        public Guid id { get; set; }
        public DateTime expires;
    }
}
