using AutoMapper;
using MasterConfiguration;
using PSCalendarContract.Dto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dto = PSCalendarContract.Dto;

namespace PSCalendarBL
{
    public class Calendar
    {

        private string ConnectionString
        {
            get
            {
                string serverName = MConfiguration.Configuration["ServerName"];
                string dbName = MConfiguration.Configuration["DatabaseName"];
                string connString = ConnectionStringHelper.ConnectionString.GetSqlEntityFrameworkConnectionString(serverName, dbName, "Calendar");
                return connString;
            }
        }

        static Calendar()
        {
            AutoMapperConfiguration.Configure();
        }

        public Calendar()
        {

        }

        private PSCalendarDB.CalendarEntities entities;
        private PSCalendarDB.CalendarEntities Entities
        {
            get
            {
                if (entities == null)
                {
                    entities = new PSCalendarDB.CalendarEntities(ConnectionString);

                }
                return entities;
            }
        }

        public void AddEvent(dto.Event @event)
        {

            PSCalendarDB.Event insert = Mapper.Map<dto.Event, PSCalendarDB.Event>(@event);
            insert.EventId = GetLastId();
            Entities.Event.Add(insert);
            Entities.Entry(insert).State = EntityState.Added;
            Entities.SaveChanges();
        }

        public int GetLastId()
        {
            var @event=Entities.Event.OrderByDescending(x=>x.EventId).FirstOrDefault();
            if (@event == null)
            {
                return 0;
            }
            else
            {
                return @event.EventId+1;
            }
        }

        public void ChangeEvent(dto.Event @event)
        {
            PSCalendarDB.Event update = Mapper.Map<dto.Event, PSCalendarDB.Event>(@event);
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

        public void AddPeriodEveent(dto.PeriodicEvent periodEvent)
        {

        }

        public List<dto.Event> GetEvents(DateTime start, DateTime end)
        {


            List<PSCalendarDB.Event> resultDb = (from i in Entities.Event
                                                 where start <= i.Date && i.Date <= end
                                                  select i).ToList();
            List<dto.Event> result = Mapper.Map<List<PSCalendarDB.Event>, List<dto.Event>>(resultDb);
            return result;
        }

        public List<dto.PeriodicEvent> GetPeriodEvents(DateTime start, DateTime end)
        {
            return new List<dto.PeriodicEvent>();
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
