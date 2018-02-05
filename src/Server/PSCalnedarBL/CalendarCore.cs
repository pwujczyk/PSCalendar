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
        public void AddEvent(Event @event)
        {
            PSCalendarDB.Event insert = Mapper.Map<Event, PSCalendarDB.Event>(@event);
            insert.EventId = GetLastId();
            insert.EventGuid = Guid.NewGuid();
            Entities.Event.Add(insert);
            Entities.Entry(insert).State = EntityState.Added;
            Entities.SaveChanges();
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
        }

        public void AddPeriodEveent(PeriodicEvent periodEvent)
        {

        }

        public List<Event> GetEvents(DateTime start, DateTime end)
        {
            List<PSCalendarDB.Event> resultDb = (from i in Entities.Event
                                                 where start <= i.Date && i.Date <= end
                                                 select i).ToList();
            List<Event> result = Mapper.Map<List<PSCalendarDB.Event>, List<Event>>(resultDb);
            return result;
        }

        public List<PeriodicEvent> GetPeriodEvents(DateTime start, DateTime end)
        {
            return new List<PeriodicEvent>();
        }

        public bool Delete(int id)
        {

            var c = new Event() { EventsId = id };
            Entities.Entry(c).State = EntityState.Deleted;
            Entities.SaveChanges();
            return true;
        }
    }
}
