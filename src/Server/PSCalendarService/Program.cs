using System.ServiceProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PSCalendarService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {

//#if DEBUG
//            Console.Write("pawel");
//            PSCalendarService instance = new PSCalendarService();
//            instance.OnDebug();
//            Console.Read();
//            Thread.Sleep(1000000);
//#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new PSCalendarService() 
            };
            ServiceBase.Run(ServicesToRun);
//#endif


        }
    }
}
