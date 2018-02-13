using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSCalendarContract.Dto
{
    public class SyncAccountEvent
    {
        public int SyncAccountEventId { get; set; }
        public Guid EventGuid { get; set; }
        public int SyncAccountId { get; set; }
        public string GoogleCalendarEventId { get; set; }
        public bool ToBeDeleted { get; set; }
        public bool Deleted { get; set; }
    }
}
