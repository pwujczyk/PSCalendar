using MasterConfiguration;
using PSCalendarBL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSCalendarBL
{
    public abstract class CalendarBase
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

        static CalendarBase()
        {
            AutoMapperConfiguration.Configure();
        }


        private PSCalendarDB.CalendarEntities entities;
        protected PSCalendarDB.CalendarEntities Entities
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

    }
}
