using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSCalendarContract;
using System.Data;
using PSCalendarContract.Dto;
using System.ServiceModel;
using SyncGmailCalendar;
using PSCalendarBL;

namespace PSCalendarServer
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class CalendarServer : ICalendar
    {
        private Calendar Calendar = new Calendar();

        public void GetDate() { }

        public void AddEvent(Event @event)
        {
            Calendar.AddEvent(@event);
        }

        public void ChangeEvent(Event @event)
        {
            Calendar.ChangeEvent(@event);
        }

        public void AddPeriodEveent(PeriodicEvent periodEvent)
        {

        }

        public List<Event> GetEvents(DateTime start, DateTime end)
        {
            return Calendar.GetEvents(start, end);
        }

        public List<PeriodicEvent> GetPeriodEvents(DateTime start, DateTime end)
        {
            return Calendar.GetPeriodEvents(start, end);
        }

        public bool Delete(int id)
        {
            return Calendar.Delete(id);
        }

        public void Sync()
        {
            SyncGoogleCalendar sync = new SyncGoogleCalendar();
            sync.Sync("pwujczyk@gmail.com");
        }
    }
}
