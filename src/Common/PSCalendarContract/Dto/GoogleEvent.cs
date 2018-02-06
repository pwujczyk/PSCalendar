using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSCalendarContract.Dto
{
    public class GoogleEvent: Event
    {
        public string GoogleCalendarEventId { get; set; }
        public bool SyncAccountTobeDeleted { get; set; }
        public bool SyncAccountDeleted { get; set; }
    }
}
