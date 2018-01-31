using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSCalendarContract;
using dto = PSCalendarContract.Dto;
using AutoMapper;
using System.Data;
using PSCalendarContract.Dto;
using System.ServiceModel;

namespace PSCalendarServer
{
    [ServiceBehavior(IncludeExceptionDetailInFaults=true)]
    public class Calendar : ICalendar
    {
        public Calendar()
        {
            AutoMapperConfiguration.Configure();
        }

        public void GetDate() { }

        public void AddEvent(dto.Event @event)
        {

            Event insert = Mapper.Map<dto.Event, Event>(@event);
            PowerShellEntities entities = new PowerShellEntities();

            entities.Events.Add(insert);
            entities.Entry(insert).State = EntityState.Added;
            entities.SaveChanges();
        }

        public void ChangeEvent(dto.Event @event)
        {
            Event update = Mapper.Map<dto.Event, Event>(@event);
            PowerShellEntities entities = new PowerShellEntities();
            var eventUpdate = entities.Events.SingleOrDefault(x => x.EventsId == update.EventsId);
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
            entities.Entry(eventUpdate).State = EntityState.Modified;
            entities.SaveChanges();
        }

        public void AddPeriodEveent(dto.PeriodicEvent periodEvent)
        {

        }

        public List<dto.Event> GetEvents(DateTime start, DateTime end)
        {

            PowerShellEntities entities = new PowerShellEntities();
            List<Event> resultDb = (from i in entities.Events
                                    where start <= i.Date && i.Date <= end
                                    select i).ToList();
            List<dto.Event> result = Mapper.Map<List<Event>, List<dto.Event>>(resultDb);
            return result;
        }

        public List<dto.PeriodicEvent> GetPeriodEvents(DateTime start, DateTime end)
        {
            return new List<dto.PeriodicEvent>();
        }

        public bool Delete(int id)
        {
            PowerShellEntities enitties = new PowerShellEntities();

            var c = new Event() { EventsId = id };
            enitties.Entry(c).State = EntityState.Deleted;
            enitties.SaveChanges();
            return true;
        }
    }
}
