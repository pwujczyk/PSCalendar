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
            //pw: master configuration
            string serverName =@".\SQL2019";
            string dbName = "PawelPT";

            DBUpPT.DBUp dBUp = new DBUpPT.DBUp("gc");
            Assembly assembly = Assembly.GetExecutingAssembly();
            dBUp.PerformUpdate(serverName, dbName, assembly, false);
        }
    }
}
