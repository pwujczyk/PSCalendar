using Google.Apis.Calendar.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncGmailCalendar
{
    public class CalendarSyncGoogle
    {
        GoogleCalendarAPI SyncGoogleCalendar = new GoogleCalendarAPI();
        PSCalendarBL.CalendarCore CalendarCoreBL = new PSCalendarBL.CalendarCore();
        PSCalendarBL.CalendarSync CalendarSyncBL = new PSCalendarBL.CalendarSync();

        static CalendarSyncGoogle()
        {
            AutomapperConfiguration.Configure();
        }

        public void Sync(string account, DateTime start, DateTime end)
        {             
            var psEventsWithAdditionalInfo = CalendarSyncBL.GetSyncEvents(start, end);
            var googleCalendarEvents = SyncGoogleCalendar.GetGoogleCalendarEvents(account, start, end);

            foreach (var item in psEventsWithAdditionalInfo)
            {
                if (item.GoogleCalendarEventId == null)
                {
                    AddEventToGoogleCalendar(account, item);
                }
                else
                {
                    var googleCalendarEvent = SyncGoogleCalendar.GetEvent(account, item.GoogleCalendarEventId);
                    var lastSyncAccountLogItemModyficationDate = CalendarSyncBL.GetLastSyncAccountLogItemModyficationDate(item.EventGuid);
                    if (googleCalendarEvent.Updated > lastSyncAccountLogItemModyficationDate)
                    {
                        UpdateEventInPSTable(googleCalendarEvent);
                    }

                    if (googleCalendarEvent.Updated < lastSyncAccountLogItemModyficationDate)
                    {
                        UpdateEventInGoogleCalendar(account, item, googleCalendarEvent);
                    }

                    CalendarSyncBL.UpdateLogItem(item.EventGuid, googleCalendarEvent.Updated.Value);
                }
            }
        }

        private void UpdateEventInGoogleCalendar(string account, PSCalendarContract.Dto.GoogleEvent item, Event googleCalendarEvent)
        {
            SyncGoogleCalendar.UpdateEvent(account, item, googleCalendarEvent.Id);
        }

        private void UpdateEventInPSTable(Event googleCalendarEvent)
        {
            var mapper = AutomapperConfiguration.dtoConfig.CreateMapper();
            PSCalendarContract.Dto.Event @event = mapper.Map<Event, PSCalendarContract.Dto.Event>(googleCalendarEvent);
            CalendarCoreBL.ChangeEvent(@event);
        }

        private void AddEventToGoogleCalendar(string account, PSCalendarContract.Dto.GoogleEvent item)
        {
            var googleCalendarevent = SyncGoogleCalendar.AddEvent(account, item);
            UpdateSyncAccountEvent(account, item, googleCalendarevent.Id);
            CalendarSyncBL.UpdateLogItem(item.EventGuid);
        }

        private void UpdateSyncAccountEvent(string acccount, PSCalendarContract.Dto.GoogleEvent item, string googleCalendarEventId)
        {
            CalendarSyncBL.UpdateSyncAccountEvent(acccount, item, googleCalendarEventId);
        }


        //private void GetGoogleCalendarEventsAndDisplay(string account, DateTime start, DateTime end)
        //{
        //    var events = SyncGoogleCalendar.GetGoogleCalendarEvents(account, start, end);
        //    DisplayEvents(events);
        //}

        private static void DisplayEvents(Events events)
        {
            Console.WriteLine("Upcoming events:");
            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    string when = eventItem.Start.DateTime.ToString();
                    if (String.IsNullOrEmpty(when))
                    {
                        when = eventItem.Start.Date;
                    }
                    Console.WriteLine("{0} ({1})", eventItem.Summary, when);
                }
            }
            else
            {
                Console.WriteLine("No upcoming events found.");
            }
        }
    }
}
