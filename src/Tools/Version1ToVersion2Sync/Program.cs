using PSCalendarBL;
using PSCalendarTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Version1ToVersion2Sync
{
    class Program
    {
        static void Main(string[] args)
        {






            CalendarCore core = new CalendarCore();

            PowerShellEntities entities = new PowerShellEntities();
            var events = entities.Events.OrderBy(x => x.Date).ToList();
            for (int j = 0; j < events.Count; j++)
            {
                //int j = i;
                PSCalendarContract.Dto.Event @event = new PSCalendarContract.Dto.Event();
                @event.Name = events[j].Name;
                @event.Type = events[j].Type.TryParse<PSCalendarContract.Dto.EventType>();
                if (events[j].Type == "Accenture")
                {
                    @event.Type = PSCalendarContract.Dto.EventType.PawelWork;
                }

                if (events[j].Type == "PawelPC")
                {
                    @event.Type = PSCalendarContract.Dto.EventType.Pawel;
                }

                if (events[j].Type == "BRE")
                {
                    @event.Type = PSCalendarContract.Dto.EventType.PawelWork;
                }

                @event.StartDate = @event.EndDate = events[j].Date;

                if (events[j].Date.TimeOfDay.TotalSeconds > 0)
                {
                    @event.EndDate = events[j].Date.AddHours(1);
                }

                if (events[j + 1].Name == events[j].Name)
                {
                    @event.EndDate = FindLastDay(events, ref j);
                }
                core.AddEvent(@event);
            }
        }

        private static DateTime FindLastDay(List<Events> events, ref int j)
        {
            var @event = events[j];

            if (j + 1 >= events.Count)
            {
                return @event.Date;
            }

            var nextEvent = events[j + 1];
            if (@event.Name == nextEvent.Name && @event.Date.Day +1== nextEvent.Date.Day )
            {
                j = j + 1;
                return FindLastDay(events, ref j);
            }
            else
            {
                return @event.Date;
            }
            throw new Exception("WTF");
        }
    }
}
