using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSCalendarServiceDebugRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new PSCalendarService.PSCalendarService();
            service.OnDebug();
            Console.ReadLine();
        }
    }
}
