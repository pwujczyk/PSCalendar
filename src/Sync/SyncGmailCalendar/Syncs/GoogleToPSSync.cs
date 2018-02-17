using Google.Apis.Calendar.v3.Data;
using PSCalendarContract.Dto;
using PSCalendarTools;
using SyncGmailCalendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSCalendarSyncGoogle.Syncs
{
    class GoogleToPSSync : SyncBase
    {
        List<GoogleEvent> PsEventsWithAdditionalInfo;

        public GoogleToPSSync(string account, DateTime start, DateTime end, Dictionary<EventType, string> calendarList)
             : base(account, start, end, calendarList)
        {
            PsEventsWithAdditionalInfo = CalendarSyncBL.GetSyncEvents(Account, Start, End);
        }


        public void Sync()
        {
            foreach (var item in CalendarList)
            {
                SyncCalendar(item.Key);
            }
        }
        private void SyncCalendar(EventType eventType)
        //private void SyncCalendar(string calendarId)
        {
            string calendarId = GetCalendarId(eventType);
            var googleCalendarEvents = SyncGoogleCalendarAPI.GetGoogleCalendarEvents(Account, Start, End, calendarId);
            foreach (var googleEvent in googleCalendarEvents.Items)
            {
                if (EventExistsInPSTable(googleEvent))
                {
                    if (GoogleEventDeleted(googleEvent))
                    {
                        DeleteEvent(googleEvent);
                    }
                    else
                    {
                        if (EventIsInDifferentCalendar(googleEvent, calendarId))
                        {
                            MoveEvent(calendarId, eventType, googleEvent);
                        }

                        if (GoogleEventIsMoreUpdated(googleEvent))
                        {

                            UpdateEvent(googleEvent);
                        }
                    }
                }
                else
                {
                    if (GoogleEventDeleted(googleEvent) == false)
                    {
                        AddGoogleEventToPSTable(calendarId, googleEvent);
                    }
                }
            }
        }

        private bool GoogleEventIsMoreUpdated(Google.Apis.Calendar.v3.Data.Event googleEvent)
        {
            var psGoogleEvent = GetGoogleEvent(googleEvent.Id);
            var lastSyncAccountLogItemModyficationDate = CalendarSyncBL.GetLastSyncAccountLogItemModyficationDate(psGoogleEvent.EventGuid);
            var r = googleEvent.Updated.Value.TrimMilliseconds() > lastSyncAccountLogItemModyficationDate.TrimMilliseconds();
            return r;
        }

        private void DeleteEvent(Google.Apis.Calendar.v3.Data.Event googleEvent)
        {
            PSCalendarContract.Dto.GoogleEvent @event = CalendarSyncBL.GetEvent(googleEvent.Id);
            CalendarCoreBL.Delete(@event.EventGuid);
            CalendarSyncBL.SyncAccountEventMarkAsDeleted(@event.GoogleCalendarEventId);

            CalendarSyncBL.UpdateLogItem(@event.EventGuid, googleEvent.Updated.Value);
        }

        private bool GoogleEventDeleted(Google.Apis.Calendar.v3.Data.Event googleEvent)
        {
            var r = googleEvent.Status == "cancelled";
            return r;
        }


        private void UpdateEvent(Google.Apis.Calendar.v3.Data.Event googleEvent)
        {
            var psGoogleEvent = GetGoogleEvent(googleEvent.Id);
            var lastSyncAccountLogItemModyficationDate = CalendarSyncBL.GetLastSyncAccountLogItemModyficationDate(psGoogleEvent.EventGuid);
            if (googleEvent.Updated.Value.TrimMilliseconds() > lastSyncAccountLogItemModyficationDate.TrimMilliseconds())
            {
                UpdateEventInPSTable(googleEvent, this.Account);
                CalendarSyncBL.UpdateLogItem(psGoogleEvent.EventGuid, googleEvent.Updated.Value);
            }
        }


        private bool EventIsInDifferentCalendar(Google.Apis.Calendar.v3.Data.Event googleEvent, string calendarId)
        {
            var b = PsEventsWithAdditionalInfo.Any(x => x.GoogleCalendarEventId == googleEvent.Id && x.GoogleCalendarId != calendarId);
            return b;
        }

        private GoogleEvent GetGoogleEvent(string googleId)
        {
            var r = PsEventsWithAdditionalInfo.SingleOrDefault(x => x.GoogleCalendarEventId == googleId);
            return r;
        }

        private bool EventExistsInPSTable(Google.Apis.Calendar.v3.Data.Event googleEvent)
        {
            return PsEventsWithAdditionalInfo.Any(x => x.GoogleCalendarEventId == googleEvent.Id);
        }

        private void MoveEvent(string calendarId, EventType eventType, Google.Apis.Calendar.v3.Data.Event googleEvent)
        {
            var psGoogleEvent = GetGoogleEvent(googleEvent.Id);
            CalendarSyncBL.UpdateGoogleCalendar(this.Account, psGoogleEvent.EventGuid, eventType, calendarId);
            //we are not updating date, as we want to perform update of elements
            //CalendarSyncBL.UpdateLogItem(psGoogleEvent.EventGuid, googleEvent.Updated.Value);
        }

        private void AddGoogleEventToPSTable(string calendarId, Google.Apis.Calendar.v3.Data.Event googleEvent)
        {
            PSCalendarContract.Dto.Event @event = ConvertEvent(googleEvent);
            @event.Type = this.CalendarList.Single(x => x.Value == calendarId).Key;
            Guid eventGuid = CalendarCoreBL.AddEvent(@event);
            CalendarSyncBL.AddSyncAccountEvent(Account, eventGuid, googleEvent.Id, calendarId);
            CalendarSyncBL.UpdateLogItem(eventGuid, googleEvent.Updated.Value);
        }

        private PSCalendarContract.Dto.Event ConvertEvent(Google.Apis.Calendar.v3.Data.Event googleEvent)
        {
            var mapper = AutomapperConfiguration.dtoConfig.CreateMapper();
            PSCalendarContract.Dto.Event @event = mapper.Map<Google.Apis.Calendar.v3.Data.Event, PSCalendarContract.Dto.Event>(googleEvent);
            return @event;
        }
    }
}
