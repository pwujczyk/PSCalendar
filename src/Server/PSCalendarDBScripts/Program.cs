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
            MConfiguration.SetConfigurationFileName("Configuration.xml");
            string serverName = MConfiguration.Configuration["ServerName"];
            string dbName = MConfiguration.Configuration["DatabaseName"];

            DBUpPT.DBUp dBUp = new DBUpPT.DBUp("gc");
            Assembly assembly = Assembly.GetExecutingAssembly();
            dBUp.PerformUpdate(serverName, dbName, assembly, false);
        }
    }
}
