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

namespace PSCalnedarBL
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

            PSCalendarDB.Events insert = Mapper.Map<dto.Event, PSCalendarDB.Events>(@event);

            Entities.Events.Add(insert);
            Entities.Entry(insert).State = EntityState.Added;
            Entities.SaveChanges();
        }

        public void ChangeEvent(dto.Event @event)
        {
            PSCalendarDB.Events update = Mapper.Map<dto.Event, PSCalendarDB.Events>(@event);
            var eventUpdate = Entities.Events.SingleOrDefault(x => x.EventsId == update.EventsId);
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


            List<PSCalendarDB.Events> resultDb = (from i in Entities.Events
                                                  where start <= i.Date && i.Date <= end
                                                  select i).ToList();
            List<dto.Event> result = Mapper.Map<List<PSCalendarDB.Events>, List<dto.Event>>(resultDb);
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
