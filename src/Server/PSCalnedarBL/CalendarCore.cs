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
            insert.NiceId = GetLastId();
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
            var @event = Entities.Event.Where(x => x.Deleted == false).OrderByDescending(x => x.NiceId).FirstOrDefault();
            if (@event == null)
            {
                return 0;
            }
            else
            {
                return @event.NiceId.Value + 1;
            }
        }

        public void ChangeEvent(Event @event)
        {
            PSCalendarDB.Event update = Mapper.Map<Event, PSCalendarDB.Event>(@event);
            var eventUpdate = Entities.Event.SingleOrDefault(x => x.NiceId == update.NiceId);
            if (update.StartDate != DateTime.MinValue)
            {
                eventUpdate.StartDate = update.StartDate;
            }
            if (update.EndDate != DateTime.MinValue)
            {
                eventUpdate.EndDate = update.EndDate;
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

        public List<Event> GetActiveEvents(DateTime start, DateTime end)
        {
            List<PSCalendarDB.Event> resultDb = (from i in Entities.Event
                                                 where start <= i.StartDate && i.StartDate <= end && i.Deleted == false
                                                 select i).OrderBy(i => i.NiceId).ToList();
            List<Event> result = Mapper.Map<List<PSCalendarDB.Event>, List<Event>>(resultDb);
            return result;
        }

        public List<Event> GetAllEvents(DateTime start, DateTime end)
        {
            List<PSCalendarDB.Event> resultDb = (from i in Entities.Event
                                                 where start <= i.StartDate && i.StartDate <= end
                                                 select i).OrderBy(i => i.NiceId).ToList();
            List<Event> result = Mapper.Map<List<PSCalendarDB.Event>, List<Event>>(resultDb);
            return result;
        }

        public List<PeriodicEvent> GetPeriodEvents(DateTime start, DateTime end)
        {
            return new List<PeriodicEvent>();
        }

        public bool Delete(int id)
        {
            var @event = Entities.Event.Where(x => x.Deleted == false).Single(x => x.NiceId == id);
            Entities.DeleteEventByEventId(@event.EventGuid);
            return true;
        }

        public bool Delete(Guid guid)
        {
            Entities.DeleteEventByEventId(guid);
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
