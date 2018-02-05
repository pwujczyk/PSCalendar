using AutoMapper;
using MasterConfiguration;
using PSCalendarContract.Dto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSCalendarBL
{
    public class CalendarCore : CalendarBase
    {
        public event EventHandler<Guid> PSCalendarEventChanged;

        public Guid AddEvent(Event @event)
        {
            return AddEvent(@event, DateTime.Now);
        }

        public Guid AddEvent(Event @event, DateTime updateDate)
        {
            PSCalendarDB.Event insert = Mapper.Map<Event, PSCalendarDB.Event>(@event);
            insert.EventId = GetLastId();
            insert.EventGuid = Guid.NewGuid();
            Entities.Event.Add(insert);
            Entities.Entry(insert).State = EntityState.Added;
            Entities.SaveChanges();
            UpdateSyncLogItem(insert.EventGuid, updateDate);
            return insert.EventGuid;
        }

        private void UpdateSyncLogItem(Guid guid, DateTime udpateDate)
        {

            new CalendarSync().UpdateLogItem(guid, udpateDate);
        }

        public int GetLastId()
        {
            var @event = Entities.Event.OrderByDescending(x => x.EventId).FirstOrDefault();
            if (@event == null)
            {
                return 0;
            }
            else
            {
                return @event.EventId + 1;
            }
        }

        public void ChangeEvent(Event @event)
        {
            PSCalendarDB.Event update = Mapper.Map<Event, PSCalendarDB.Event>(@event);
            var eventUpdate = Entities.Event.SingleOrDefault(x => x.EventId == update.EventId);
            if (update.Date != DateTime.MinValue)
            {
                eventUpdate.Date = update.Date;
            }
            if (!string.IsNullOrEmpty(update.Name))
            {
                eventUpdate.Name = update.Name;
            }
            if (update.Type != EventType.None.ToString())
            {
                eventUpdate.Type = update.Type;
            }
            Entities.Entry(eventUpdate).State = EntityState.Modified;
            Entities.SaveChanges();
            UpdateSyncLogItem(eventUpdate.EventGuid, DateTime.Now);
        }

        public void AddPeriodEveent(PeriodicEvent periodEvent)
        {

        }

        public List<Event> GetEvents(DateTime start, DateTime end)
        {
            List<PSCalendarDB.Event> resultDb = (from i in Entities.Event
                                                 where start <= i.Date && i.Date <= end
                                                 select i).OrderBy(i => i.EventId).ToList();
            List<Event> result = Mapper.Map<List<PSCalendarDB.Event>, List<Event>>(resultDb);
            return result;
        }


        public List<PeriodicEvent> GetPeriodEvents(DateTime start, DateTime end)
        {
            return new List<PeriodicEvent>();
        }

        public bool Delete(int id)
        {
            var c = new Event() { EventId = id };
            Entities.Entry(c).State = EntityState.Deleted;
            Entities.SaveChanges();
            return true;
        }

        private void OnPSCalendarEventChanged(Guid eventArgs)
        {
            if (PSCalendarEventChanged != null)
            {
                PSCalendarEventChanged.Invoke(this, eventArgs);
            }
        }
    }
}
