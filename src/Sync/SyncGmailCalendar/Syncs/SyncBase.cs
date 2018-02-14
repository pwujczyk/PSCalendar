using SyncGmailCalendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSCalendarSyncGoogle.Syncs
{
    public class SyncBase
    {
        protected DateTime Start, End;
        protected string Account;

        protected PSCalendarBL.CalendarCore CalendarCoreBL = new PSCalendarBL.CalendarCore();
        protected PSCalendarBL.CalendarSync CalendarSyncBL = new PSCalendarBL.CalendarSync();
        protected GoogleCalendarAPI SyncGoogleCalendarAPI = new GoogleCalendarAPI();

        protected Dictionary<PSCalendarContract.Dto.EventType, string> CalendarList;

        protected string GetCalendarId(PSCalendarContract.Dto.EventType type)
        {
            return this.CalendarList.Single(x => x.Key == type).Value;
        }

        public SyncBase(string account, DateTime start, DateTime end, Dictionary<PSCalendarContract.Dto.EventType, string> calendarList)
        {
            this.Start = start;
            this.End = end;
            this.Account = account;
            this.CalendarList = calendarList;
        }

    }
}
