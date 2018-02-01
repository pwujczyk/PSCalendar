using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSCalendarContract.Dto
{
    public class PeriodicEvent
    {
        public int PeriodicEventsId { get; set; }
        public string Name { get; set; }
        public int PeriodType { get; set; }
    }
}
