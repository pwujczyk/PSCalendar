using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
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
            AutoMapperConfiguration.Configure();
            host = new ServiceHost(typeof(Calendar));
            host.Open();
        }

        protected override void OnStop()
        {
            host.Close();
        }
    }
}
