
using PSCalendarContract;
using PSCalendarContract.Dto;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace PSCalendar
{
    [Cmdlet(VerbsCommon.Get, "Calendar")]
    public class PSCalendarCmdlet : PSCmdlet
    {

        private ICalendar Client
        {
            get
            {
                ChannelFactory<ICalendar> factory = new ChannelFactory<ICalendar>(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:9005/"));
                ICalendar proxy = factory.CreateChannel();
                return proxy;


                //Binding binding = new BasicHttpBinding();
                //EndpointAddress address = new EndpointAddress("http://localhost:9004/");
                //CalendarClient client = new CalendarClient(binding, address);
                //client.Open();
                //return client;
            }
        }


        private static string Spacer = "".PadLeft(4);

        [Parameter]
        public string Add { get; set; }

        [Parameter]
        public string Date { get; set; }

        [Parameter]
        public EventType Type { get; set; }

        [Parameter]
        public int? ShowMonth { get; set; }

        [Parameter]
        public SwitchParameter Help { get; set; }

        [Parameter]
        public int? Delete { get; set; }

        [Parameter]
        public int? Change { get; set; }

        protected override void BeginProcessing()
        {

            if (Help.IsPresent)
            {
                ShowHelp();
                return;
            }

            if (this.Delete.HasValue)
            {
                DeleteItem(this.Delete);
                ShowCurrentMonth();
                return;
            }


            if (this.Change.HasValue)
            {
                ChangeItem(this.Change);
                ShowCurrentMonth();
                return;
            }

            if (!string.IsNullOrEmpty(this.Add) && !string.IsNullOrEmpty(this.Date))
            {
                AddNewEvent(this.Add, DateTime.Parse(this.Date), this.Type);
            }

            if (ShowMonth.HasValue)
            {
                ShowSelectedMonth(DateTime.Now.AddMonths(ShowMonth.Value));
            }
            else
            {
                ShowCurrentMonth();
            }

            base.BeginProcessing();
        }

        private void ChangeItem(int? nullable)
        {
            DateTime dt = this.Date == null ? DateTime.MinValue : DateTime.Parse(this.Date);
            Client.ChangeEvent(new Event { EventsId = nullable.Value, Date = dt, Name = this.Add, Type = this.Type });
            (Client as ICommunicationObject).Close();
        }

        private void DeleteItem(int? nullable)
        {
            Client.Delete(nullable.Value);
            (Client as ICommunicationObject).Close();
        }

        private void ShowHelp()
        {
            Console.WriteLine("Add");
            Console.WriteLine("Date");
            Console.WriteLine("ShowMonth");
            Console.WriteLine("Help");
            Console.WriteLine("Add");
            Console.WriteLine("Delete");
            Console.WriteLine("Change");

            foreach (var item in Enum.GetNames(typeof(EventType)))
            {
                Console.WriteLine(string.Format("EventType: {0}", item));
            }
        }

        private void AddNewEvent(string name, DateTime date, EventType type)
        {
            Client.AddEvent(new Event { Date = date, Name = name, Type = type });
            (Client as ICommunicationObject).Close();
        }


        private void ShowCurrentMonth()
        {
            ShowSelectedMonth(DateTime.Now);
        }

        private void ShowSelectedMonth(DateTime date)
        {
            ShowCalendarRange(date.GetFirstMonthDay(), date.GetLastMonthDay());
        }

        private List<Event> GetEvents(DateTime start, DateTime end)
        {
            return Client.GetEvents(start, end).ToList();
        }

        private void WriteEvent(Stack<Event> eventsStack)
        {
            if (eventsStack.Count > 0)
            {
                Event e = eventsStack.Pop();
                Console.ForegroundColor =
                    PSCalendar.Extensions.CommonExtensions.ParseEnum<System.ConsoleColor>(e.Color.ToString());
                Console.Write(string.Format("{0}. {1}    {2}", e.EventsId, e.Date, e.Name));
                Console.ResetColor();

            }
        }

        private void ShowCalendarRange(DateTime start, DateTime end)
        {
            var eventsList = GetEvents(start, end);
            Stack<Event> eventsStack = new Stack<Event>(eventsList.OrderByDescending(x => x.Date));
            ShowMonthTitle(start);
            FillEmptyValues(start);
            DateTime lastItem = DateTime.Now;
            foreach (var item in EachDay(start, end))
            {
                WriteDayNumber(eventsList, item);
                if (item.DayOfWeek == DayOfWeek.Sunday)
                {
                    Console.Write("\t");
                    WriteEvent(eventsStack);
                    Console.Write(Environment.NewLine);
                }
                lastItem = item;
            }


            if (eventsStack.Count > 0)
            {
                for (int i = lastItem.DayOfWeek == DayOfWeek.Sunday ? 0 : (int)lastItem.DayOfWeek; i <= 7; i++)
                {
                    Console.Write("".PadLeft(4));
                }

                WriteEvent(eventsStack);
            }

            Console.Write(Environment.NewLine);

            for (int i = eventsStack.Count; i > 0; i--)
            {
                Console.Write("\t"); Console.Write("\t"); Console.Write("\t"); Console.Write("\t");
                WriteEvent(eventsStack);
                Console.Write(Environment.NewLine);
            }
            Console.WriteLine("");
        }

        private static void WriteDayNumber(List<Event> eventsList, DateTime item)
        {
            var @event = eventsList.Where(x => x.Date.ToShortDateString() == item.ToShortDateString()).ToList();
            if (@event.Count > 0)
            {
                if (@event.Count == 1)
                {
                    Console.ForegroundColor =
                        PSCalendar.Extensions.CommonExtensions.ParseEnum<System.ConsoleColor>(@event[0].Color.ToString());
                }
                else
                {
                    Console.ForegroundColor = System.ConsoleColor.Red;
                }
            }
            if (item.ToShortDateString() == DateTime.Now.ToShortDateString())
            {
                Console.Write(string.Concat(".", item.Day.ToString()).PadLeft(4));
            }
            else
            {
                Console.Write(item.Day.ToString().PadLeft(4));
            }
            Console.ResetColor();
        }

        private void ShowMonthTitle(DateTime start)
        {
            Console.WriteLine(string.Format("[{0}] ", start.Month.ToString().PadLeft(2,'0')) + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(start.Month));
        }

        private void FillEmptyValues(DateTime start)
        {
            int delta = start.DayOfWeek - DayOfWeek.Sunday;
            if (delta == 0)
            {
                delta = 7;
            }
            for (int i = 0; i < delta - 1; i++)
            {
                Console.Write(Spacer);
            }
        }

        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }
    }
}
