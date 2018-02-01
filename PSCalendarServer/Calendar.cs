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
using MasterConfiguration;
using System.Data.Entity;
using DB;

namespace PSCalendarServer
{
    [ServiceBehavior(IncludeExceptionDetailInFaults=true)]
    public class Calendar : ICalendar
    {

        private string ConnectionString
        {
            get
            {
                string serverName = MConfiguration.Configuration["ServerName"];
                string dbName = MConfiguration.Configuration["DatabaseName"];
                string connString=ConnectionStringHelper.ConnectionString.GetSqlEntityFrameworkConnectionString(serverName, dbName, "Calendar");
                return connString;
            }
        }

        public Calendar()
        {
            AutoMapperConfiguration.Configure();
        }

        private CalendarEntities Entities
        {
            get
            {
                return new CalendarEntities(ConnectionString);
            }
        }

        public void GetDate() { }

        public void AddEvent(dto.Event @event)
        {
            DB.Events insert = Mapper.Map<dto.Event, DB.Events>(@event);
            CalendarEntities entities = Entities;

            entities.Events.Add(insert);
            entities.Entry(insert).State = EntityState.Added;
            entities.SaveChanges();
        }

        public void ChangeEvent(dto.Event @event)
        {
            DB.Events update = Mapper.Map<dto.Event, DB.Events>(@event);
            CalendarEntities entities = Entities;
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

            CalendarEntities entities = Entities;
            List<DB.Events> resultDb = (from i in entities.Events
                                    where start <= i.Date && i.Date <= end
                                    select i).ToList();
            List<dto.Event> result = Mapper.Map<List<DB.Events>, List<dto.Event>>(resultDb);
            return result;
        }

        public List<dto.PeriodicEvent> GetPeriodEvents(DateTime start, DateTime end)
        {
            return new List<dto.PeriodicEvent>();
        }

        public bool Delete(int id)
        {
            CalendarEntities entities = Entities;

            var c = new Event() { EventsId = id };
            entities.Entry(c).State = EntityState.Deleted;
            entities.SaveChanges();
            return true;
        }
    }
}
