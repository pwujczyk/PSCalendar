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
        public void Sync(DateTime start, DateTime end)
        {
            CalendarSyncGoogle sync = new CalendarSyncGoogle();
            //var start = DateTime.Now;//.GetFirstMonthDay();
            //var end = DateTime.Now;//.GetLastMonthDay();
            sync.Sync("pwujczyk@gmail.com",start,end);
        }

        public void AddSyncAccount(string email)
        {
            CalendarSync calendarsync = new CalendarSync();
            calendarsync.AddSyncAccount(email);
        }
    }
}
