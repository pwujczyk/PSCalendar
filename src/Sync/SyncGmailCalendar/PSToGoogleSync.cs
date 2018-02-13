using IndexList;
using PSCalendarContract.Dto;
using PSCalendarTools;
using SyncGmailCalendar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PSCalendarSyncGoogle
{
    public class PSToGoogleSync
    {

        DateTime Start, End;
        string Account;

        PSCalendarBL.CalendarCore CalendarCoreBL = new PSCalendarBL.CalendarCore();
        PSCalendarBL.CalendarSync CalendarSyncBL = new PSCalendarBL.CalendarSync();
        GoogleCalendarAPI SyncGoogleCalendarAPI = new GoogleCalendarAPI();

        private List<Event> ItemsToBeSync;

        Dictionary<EventType, string> CalendarList;

        IndexList<GoogleEvent> alreadyCreatedItems;
        private IndexList<GoogleEvent> AlreadyCreatedItems
        {
            get
            {
                if (alreadyCreatedItems == null)
                {
                    var SyncEvents = this.CalendarSyncBL.GetSyncEvents(Account, Start, End);

                    this.alreadyCreatedItems = new IndexList<GoogleEvent>();
                    this.alreadyCreatedItems.AddRange(SyncEvents);
                    this.alreadyCreatedItems.AddIndex<Guid>((events, selector) => events.Single(x => x.EventGuid == selector));
                }

                return this.alreadyCreatedItems;
            }
        }

        public PSToGoogleSync(string account, DateTime start, DateTime end, Dictionary<EventType, string> calendarList)
        {
            this.Start = start;
            this.End = end;
            this.Account = account;

            this.ItemsToBeSync = GetItems(start, end);

            this.CalendarList = calendarList;
        }

        public void Sync()
        {
            foreach (var item in this.ItemsToBeSync)
            {

                if (GoogleCalendarEventExists(item))
                {
                    if (ItemShouldBeDeleted(item))
                    {//delete event
                        DeleteEvent(item);
                    }
                    else //update event
                    {
                        UpdateEvent(item);
                    }
                }
                else
                {//create event
                    AddEventToGoogleCalendar(item);
                }
            }
        }

        private void UpdateEvent(Event item)
        {
            var calendarId = GetCalendarId(item.Type);
            var googleCalendarEventId = AlreadyCreatedItems[item.EventGuid].GoogleCalendarEventId;

            var googleCalendarEvent = SyncGoogleCalendarAPI.GetEvent(this.Account, googleCalendarEventId, calendarId);
            var lastSyncAccountLogItemModyficationDate = CalendarSyncBL.GetLastSyncAccountLogItemModyficationDate(item.EventGuid);

            if (googleCalendarEvent.Updated.Value.TrimMilliseconds() > lastSyncAccountLogItemModyficationDate.TrimMilliseconds())
            {
                UpdateEventInPSTable(googleCalendarEvent, this.Account);
                CalendarSyncBL.UpdateLogItem(item.EventGuid, googleCalendarEvent.Updated.Value);
            }

            if (googleCalendarEvent.Updated.Value.TrimMilliseconds() < lastSyncAccountLogItemModyficationDate.TrimMilliseconds())
            {
                UpdateEventInGoogleCalendar(this.Account, item, googleCalendarEvent, calendarId);
                CalendarSyncBL.UpdateLogItem(item.EventGuid, googleCalendarEvent.Updated.Value);
            }
        }

        private void UpdateEventInGoogleCalendar(string account, Event item, Google.Apis.Calendar.v3.Data.Event googleCalendarEvent, string calendarid)
        {
            SyncGoogleCalendarAPI.UpdateEvent(account, item, googleCalendarEvent.Id, calendarid);
        }

        private void UpdateEventInPSTable(Google.Apis.Calendar.v3.Data.Event googleEvent, string account)
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
                @event.EndDate = googleEvent.End.DateTime.Value;
                //todo: change to automapper
                CalendarCoreBL.ChangeEvent(@event);
            }
        }


        private void DeleteEvent(Event item)
        {
            var calendarId = GetCalendarId(item.Type);
            var googleCalendarEventId = AlreadyCreatedItems[item.EventGuid].GoogleCalendarEventId;

            SyncGoogleCalendarAPI.Delete(this.Account, googleCalendarEventId, calendarId);
            CalendarSyncBL.SyncAccountEventMarkAsDeleted(googleCalendarEventId, this.Account);

            var googleCalendarEvent = SyncGoogleCalendarAPI.GetEvent(this.Account, googleCalendarEventId, calendarId);
            CalendarSyncBL.UpdateLogItem(item.EventGuid, googleCalendarEvent.Updated.Value);
        }

        private bool ItemShouldBeDeleted(Event item)
        {
            var createdItem = AlreadyCreatedItems[item.EventGuid];
            return createdItem.SyncAccountTobeDeleted && createdItem.SyncAccountDeleted == false;
        }

        private GoogleEvent GetGoogleCalendarEvent(Guid guid)
        {
            return this.AlreadyCreatedItems.SingleOrDefault(x => x.EventGuid == guid);
        }

        private bool GoogleCalendarEventExists(Event item)
        {
            return GetGoogleCalendarEvent(item.EventGuid) != null;
        }

        private List<PSCalendarContract.Dto.Event> GetItems(DateTime start, DateTime end)
        {
            return this.CalendarCoreBL.GetAllEvents(start, end);
        }

        private void AddEventToGoogleCalendar(Event item)
        {
            var calendarId = GetCalendarId(item.Type);
            var googleCalendarEvent = SyncGoogleCalendarAPI.AddEvent(this.Account, item, calendarId);
            CalendarSyncBL.UpdateSyncAccountEvent(this.Account, item.EventGuid, googleCalendarEvent.Id);
            CalendarSyncBL.UpdateLogItem(item.EventGuid, googleCalendarEvent.Updated.Value);
        }


        private string GetCalendarId(PSCalendarContract.Dto.EventType type)
        {
            return this.CalendarList.Single(x => x.Key == type).Value;
        }
    }
}
