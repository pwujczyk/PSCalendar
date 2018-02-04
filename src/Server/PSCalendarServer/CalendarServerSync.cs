using PSCalendarBL;
using PSCalendarContract;
using SyncGmailCalendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSCalendarServer
{
    public partial class CalendarServer
    {
        public void Sync()
        {
            SyncGoogleCalendar sync = new SyncGoogleCalendar();
            sync.Sync("pwujczyk@gmail.com");
        }

        public void AddSyncAccount(string email)
        {
            CalendarSync calendarsync = new CalendarSync();
            calendarsync.AddSyncAccount(email);
        }
    }
}
