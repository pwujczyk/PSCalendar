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
            DBUpHelper.DBUp dBUp = new DBUpHelper.DBUp("gc");
            Assembly assembly = Assembly.GetExecutingAssembly();
            string serverName = MasterConfiguration.MConfiguration.Configuration["ServerName"];
            string dbName = MasterConfiguration.MConfiguration.Configuration["DatabaseName"];

            //todo:get from configuration
            dBUp.PerformUpdate(serverName, dbName, assembly, false);
        }
    }
}
