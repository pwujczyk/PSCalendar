using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSCalendarContract.Dto
{
    public class Event
    {
        public int EventId { get; set; }
        public Guid EventGuid { get; set; }
        public string Name { get; set; }
        public System.DateTime Date { get; set; }
        public EventType Type { get; set; }

        public ConsoleColor Color { get; set; }
    }
}
