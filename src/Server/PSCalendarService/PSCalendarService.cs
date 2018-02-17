using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using MasterConfiguration;
using PSCalendarContract;
using PSCalendarServer;

namespace PSCalendarService
{
    public partial class PSCalendarService : ServiceBase
    {
        ServiceHost host;
        public PSCalendarService()
        {
            InitializeComponent();
        }

        public void OnDebug()
        {
            
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            PSCalendarDBScripts.Program.Main(null);


            var binding = new NetTcpBinding();
            var address = MConfiguration.Configuration["Address"];
            host = new ServiceHost(typeof(CalendarServer));

            ServiceDebugBehavior debug = host.Description.Behaviors.Find<ServiceDebugBehavior>();
            if (debug == null)
            {
                host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
            }
            else
            {
                if (!debug.IncludeExceptionDetailInFaults)
                {
                    debug.IncludeExceptionDetailInFaults = true;
                }
            }

            host.AddServiceEndpoint(typeof(ICalendar), binding, address);
            host.AddServiceEndpoint(typeof(ICalendarSync), binding, address);
            host.CloseTimeout = TimeSpan.FromMinutes(20);
            host.Open();
        }

        protected override void OnStop()
        {
            host.Close();
        }
    }
}
