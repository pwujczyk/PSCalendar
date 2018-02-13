using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSCalendarContract.Dto
{
    //TODO: remove this
    public class GoogleEvent: Event
    {
        public string GoogleCalendarEventId { get; set; }
        public bool SyncAccountTobeDeleted { get; set; }
        public bool SyncAccountDeleted { get; set; }
        public string Email { get; set; }
    }
}
