using Configuration;
using ProductivityTools.MasterConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PSCalendarDBScripts
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MasterConfiguration.MConfiguration.SetConfigurationFileName("MasterConfiguration.xml");
            MasterConfiguration.MConfiguration.SetApplicationName("PSCalendar");
            string serverName = MasterConfiguration.MConfiguration["ServerName"];
            string dbName = MasterConfiguration.MConfiguration["DatabaseName"];

            ProductivityTools.DBUp dBUp = new ProductivityTools.DBUp("gc");
            Assembly assembly = Assembly.GetExecutingAssembly();
            dBUp.PerformUpdate(serverName, dbName, assembly, false);
        }
    }
}
