using Google.Apis.Calendar.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSCalendarTools;

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

        public CalendarSyncGoogle()
        {
           
        }


        public void Sync(string account, DateTime start, DateTime end)
        {
            SyncPowershellToGoogle(account, start, end);
            SyncGoogleToPowershell(account, start, end);
        }

        public void SyncPowershellToGoogle(string account, DateTime start, DateTime end)
        {
            var psEventsWithAdditionalInfo = CalendarSyncBL.GetSyncEvents(start, end);

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
                    if (googleCalendarEvent.Updated.Value.TrimMilliseconds() > lastSyncAccountLogItemModyficationDate.TrimMilliseconds())
                    {
                        UpdateEventInPSTable(googleCalendarEvent);
                        CalendarSyncBL.UpdateLogItem(item.EventGuid, googleCalendarEvent.Updated.Value);
                    }

                    if (googleCalendarEvent.Updated.Value.TrimMilliseconds() < lastSyncAccountLogItemModyficationDate.TrimMilliseconds())
                    {
                        UpdateEventInGoogleCalendar(account, item, googleCalendarEvent);
                        CalendarSyncBL.UpdateLogItem(item.EventGuid, googleCalendarEvent.Updated.Value);
                    }

                    
                }
            }
        }

        public void SyncGoogleToPowershell(string account, DateTime start, DateTime end)
        {
            var googleCalendarEvents = SyncGoogleCalendar.GetGoogleCalendarEvents(account, start, end);
            var psEventsWithAdditionalInfo = CalendarSyncBL.GetSyncEvents(start, end);

            foreach (var googleEvent in googleCalendarEvents.Items)
            {
                if (psEventsWithAdditionalInfo.Any(x => x.GoogleCalendarEventId == googleEvent.Id))
                {
                    continue;
                }
                else
                {
                    PSCalendarContract.Dto.Event @event = ConvertEvent(googleEvent);
                    Guid eventGuid=CalendarCoreBL.AddEvent(@event);
                    UpdateSyncAccountEvent(account, eventGuid, googleEvent.Id);
                    CalendarSyncBL.UpdateLogItem(eventGuid, googleEvent.Updated.Value);
                }
            }
        }

        private PSCalendarContract.Dto.Event ConvertEvent(Event googleEvent)
        {
            var mapper = AutomapperConfiguration.dtoConfig.CreateMapper();
            PSCalendarContract.Dto.Event @event = mapper.Map<Event, PSCalendarContract.Dto.Event>(googleEvent);
            return @event;
        }

        private void UpdateEventInGoogleCalendar(string account, PSCalendarContract.Dto.GoogleEvent item, Event googleCalendarEvent)
        {
            SyncGoogleCalendar.UpdateEvent(account, item, googleCalendarEvent.Id);
        }

        private void UpdateEventInPSTable(Event googleEvent)
        {
            PSCalendarContract.Dto.GoogleEvent @event = CalendarSyncBL.GetEvent(googleEvent.Id);
            @event.Name = googleEvent.Summary;
            @event.Date = googleEvent.Start.DateTime.Value;
            //todo: change to automapper
            CalendarCoreBL.ChangeEvent(@event);
        }



        private void AddEventToGoogleCalendar(string account, PSCalendarContract.Dto.GoogleEvent item)
        {
            var googleCalendarEvent = SyncGoogleCalendar.AddEvent(account, item);
            UpdateSyncAccountEvent(account, item.EventGuid, googleCalendarEvent.Id);
            CalendarSyncBL.UpdateLogItem(item.EventGuid,googleCalendarEvent.Updated.Value);
        }

        private void UpdateSyncAccountEvent(string acccount, Guid eventGuid, string googleCalendarEventId)
        {
            CalendarSyncBL.UpdateSyncAccountEvent(acccount, eventGuid, googleCalendarEventId);
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
