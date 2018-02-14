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
        public void SyncAllAcounts(DateTime start, DateTime end)
        {
            CalendarSyncGoogle sync = new CalendarSyncGoogle();
            sync.SyncAllAcounts(start, end);
        }

        public void SyncAccount(DateTime start, DateTime end, string account)
        {
            CalendarSyncGoogle sync = new CalendarSyncGoogle();
            sync.SyncAccount(account, start, end);
        }

        public void AddSyncAccount(string email)
        {
            CalendarSync calendarsync = new CalendarSync();
            calendarsync.AddSyncAccount(email);
        }

        public void AddCalendarsToAccount(string account)
        {
            CalendarSyncGoogle s = new CalendarSyncGoogle();
            s.CreateCalendars(account);
        }

        public void ClearAccount(string account, DateTime start, DateTime end)
        {
            CalendarSyncGoogle s = new CalendarSyncGoogle();
            s.ClearAccount(account,start,end);
        }
    }
}
