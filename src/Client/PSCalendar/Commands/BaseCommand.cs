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
        public abstract void InvokeCommand();
    }

    abstract class BaseCommand<T> : BaseCommand
    {
        //todo: jak zrobic wyswietlanie
        protected PSCalendarCmdlet Cmdlet;

        protected T Client
        {
            get
            {
                //pw: masterconfiguration
                string address = @"net.tcp://localhost:9007";
                NetTcpBinding netTcpBinding = new NetTcpBinding();
                netTcpBinding.CloseTimeout = TimeSpan.FromMinutes(20);
                ChannelFactory<T> factory = new ChannelFactory<T>(netTcpBinding, new EndpointAddress(address));
               
                T proxy = factory.CreateChannel();
                
                return proxy;
            }
        }

        //protected ICalendarSync Client
        //{
        //    get
        //    {
        //        string address = MasterConfiguration.MConfiguration.Configuration["Address"];
        //        ChannelFactory<ICalendarSync> factory = new ChannelFactory<ICalendarSync>(new NetTcpBinding(), new EndpointAddress(address));
        //        ICalendarSync proxy = factory.CreateChannel();
        //        return proxy;
        //    }
        //}


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

        public override void InvokeCommand()
        {
            if (this.Condition)
            {
                Invoke();
            }
        }
    }
}
