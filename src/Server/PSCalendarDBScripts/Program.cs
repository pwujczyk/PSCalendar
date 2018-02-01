using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PSCalendarDBScripts
{
    class Program
    {
        static void Main(string[] args)
        {
            DBUpHelper.DBUp dBUp = new DBUpHelper.DBUp("gc");
            Assembly assembly = Assembly.GetExecutingAssembly();
            string serverName = MasterConfiguration.MConfiguration.Configuration["ServerName"];
            string dbName= MasterConfiguration.MConfiguration.Configuration["DatabaseName"];

            dBUp.PerformUpdate(".\\sql2014", "PawelPT", assembly, true);
        }
    }
}
