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
        GoogleCalendarAPI SyncGoogleCalendarAPI = new GoogleCalendarAPI();
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
                if (EventNotExistsInGoogleCalendar(item))
                {
                    AddEventToGoogleCalendar(account, item);
                }
                else
                {
                    if (GoogleCalendarEventShouldBeDeleted(item))
                    {
                        SyncGoogleCalendarAPI.Delete(account, item.GoogleCalendarEventId);
                        CalendarSyncBL.SyncAccountEventMarkAsDeleted(item.GoogleCalendarEventId, account);
                        var googleCalendarEvent = SyncGoogleCalendarAPI.GetEvent(account, item.GoogleCalendarEventId);
                        CalendarSyncBL.UpdateLogItem(item.EventGuid, googleCalendarEvent.Updated.Value);
                    }
                    else
                    {
                        var googleCalendarEvent = SyncGoogleCalendarAPI.GetEvent(account, item.GoogleCalendarEventId);
                        var lastSyncAccountLogItemModyficationDate = CalendarSyncBL.GetLastSyncAccountLogItemModyficationDate(item.EventGuid);
                        if (googleCalendarEvent.Updated.Value.TrimMilliseconds() > lastSyncAccountLogItemModyficationDate.TrimMilliseconds())
                        {
                            UpdateEventInPSTable(googleCalendarEvent,account);
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
        }

        private static bool GoogleCalendarEventShouldBeDeleted(PSCalendarContract.Dto.GoogleEvent item)
        {
            return item.SyncAccountTobeDeleted && item.SyncAccountDeleted == false;
        }

        private static bool EventNotExistsInGoogleCalendar(PSCalendarContract.Dto.GoogleEvent item)
        {
            return item.GoogleCalendarEventId == null;
        }

        public void SyncGoogleToPowershell(string account, DateTime start, DateTime end)
        {
            var googleCalendarEvents = SyncGoogleCalendarAPI.GetGoogleCalendarEvents(account, start, end);
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
                    Guid eventGuid = CalendarCoreBL.AddEvent(@event);
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
            SyncGoogleCalendarAPI.UpdateEvent(account, item, googleCalendarEvent.Id);
        }

        private void UpdateEventInPSTable(Event googleEvent, string account)
        {
            PSCalendarContract.Dto.GoogleEvent @event = CalendarSyncBL.GetEvent(googleEvent.Id);
            if (googleEvent.Status == "cancelled")
            {
                CalendarCoreBL.Delete(@event.EventGuid);
                CalendarSyncBL.SyncAccountEventMarkAsDeleted(@event.GoogleCalendarEventId, account);
            }
            else
            {
                @event.Name = googleEvent.Summary;
                @event.StartDate = googleEvent.Start.DateTime.Value;
                //todo: change to automapper
                CalendarCoreBL.ChangeEvent(@event);
            }
        }



        private void AddEventToGoogleCalendar(string account, PSCalendarContract.Dto.GoogleEvent item)
        {
            var googleCalendarEvent = SyncGoogleCalendarAPI.AddEvent(account, item);
            UpdateSyncAccountEvent(account, item.EventGuid, googleCalendarEvent.Id);
            CalendarSyncBL.UpdateLogItem(item.EventGuid, googleCalendarEvent.Updated.Value);
        }

        private void UpdateSyncAccountEvent(string acccount, Guid eventGuid, string googleCalendarEventId)
        {
            CalendarSyncBL.UpdateSyncAccountEvent(acccount, eventGuid, googleCalendarEventId);
        }

        public void CreateCalendars(string account)
        {
            IEnumerable<string> calendars = SyncGoogleCalendarAPI.GetGoogleCalendars(account).Items.Select(x => x.Summary);
            List<string> categories= Enum.GetNames(typeof(PSCalendarContract.Dto.EventType)).ToList();
            foreach (var item in categories.Except(calendars))
            {
                if (item == PSCalendarContract.Dto.EventType.None.ToString()) continue;
                SyncGoogleCalendarAPI.CreateGoogleCalendar(account, item);
            }
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
