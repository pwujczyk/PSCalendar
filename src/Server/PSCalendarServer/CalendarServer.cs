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
    public partial class CalendarServer : ICalendar, ICalendarSync
    {
        private CalendarCore Calendar = new CalendarCore();

        public CalendarServer()
        {
            this.Calendar.PSCalendarEventChanged += Calendar_PSCalendarEventChanged;
        }

        private void Calendar_PSCalendarEventChanged(object sender, Guid e)
        {
            var CalendarSync = new CalendarSync();
            CalendarSync.UpdateLogItem(e);
        }

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

    }
}
