using PSCalendarContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PSCalendar.Commands
{
    abstract class BaseCommand
    {
        //todo: jak zrobic wyswietlanie
        protected PSCalendarCmdlet Cmdlet;

        protected ICalendar Client
        {
            get
            {
                string address = MasterConfiguration.MConfiguration.Configuration["Address"];
                ChannelFactory<ICalendar> factory = new ChannelFactory<ICalendar>(new NetTcpBinding(), new EndpointAddress(address));
                ICalendar proxy = factory.CreateChannel();
                return proxy;


                //Binding binding = new BasicHttpBinding();
                //EndpointAddress address = new EndpointAddress("http://localhost:9004/");
                //CalendarClient client = new CalendarClient(binding, address);
                //client.Open();
                //return client;
            }
        }


        public BaseCommand(PSCalendarCmdlet cmdlet)
        {
            this.Cmdlet = cmdlet;
        }


        protected abstract bool Condition { get; }

        protected abstract void Invoke();

        protected void InvokeCall(Action a)
        {
            a();
            (Client as ICommunicationObject).Close();
        }

        public void InvokeCommand()
        {
            if (this.Condition)
            {
                Invoke();
            }
        }
    }
}
