using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSCalendarContract.Dto
{
    public class Event
    {
        public int NiceId { get; set; }
        public Guid EventGuid { get; set; }
        public string Name { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public EventType Type { get; set; }
        public bool Deleted { get; set; }

        public int Color { get; set; }
    }
}
