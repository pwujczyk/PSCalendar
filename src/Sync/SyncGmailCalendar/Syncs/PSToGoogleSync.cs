using IndexList;
using PSCalendarContract.Dto;
using PSCalendarTools;
using SyncGmailCalendar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PSCalendarSyncGoogle.Syncs
{
    public class PSToGoogleSync : SyncBase
    {
        private List<Event> ItemsToBeSync;
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
            : base(account, start, end, calendarList)
        {
            this.ItemsToBeSync = GetItems(start, end);
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
                        if (ItemAlreadyDeleted(item) == false)
                        {
                            UpdateEvent(item);
                        }
                    }
                }
                else
                {//create event
                    if (EventDelteted(item) == false)
                    {
                        AddEventToGoogleCalendar(item);
                    }
                }
            }
        }

        private bool ItemAlreadyDeleted(Event item)
        {
            var createdItem = AlreadyCreatedItems[item.EventGuid];
            var r = createdItem.SyncAccountTobeDeleted && createdItem.SyncAccountDeleted;
            return r;
        }

        private bool EventDelteted(Event item)
        {
            var r = item.Deleted;
            return r;
        }

        private void UpdateEvent(Event item)
        {
            //var calendarId = GetCalendarId(item.Type);
            var googleCalendarEventId = AlreadyCreatedItems[item.EventGuid].GoogleCalendarEventId;
            var calendarId = AlreadyCreatedItems[item.EventGuid].GoogleCalendarId;

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
            //var calendarId = GetCalendarId(item.Type);
            var googleCalendarEventId = AlreadyCreatedItems[item.EventGuid].GoogleCalendarEventId;
            var calendarId = AlreadyCreatedItems[item.EventGuid].GoogleCalendarId;

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

        private GoogleEvent GetPSEvent(Guid guid)
        {
            return this.AlreadyCreatedItems.SingleOrDefault(x => x.EventGuid == guid);
        }

        private bool GoogleCalendarEventExists(Event item)
        {
            return GetPSEvent(item.EventGuid)?.GoogleCalendarEventId != null;
        }

        private List<PSCalendarContract.Dto.Event> GetItems(DateTime start, DateTime end)
        {
            return this.CalendarCoreBL.GetAllEvents(start, end);
        }

        private void AddEventToGoogleCalendar(Event item)
        {
            var calendarId = GetCalendarId(item.Type);
            var googleCalendarEvent = SyncGoogleCalendarAPI.AddEvent(this.Account, item, calendarId);
            CalendarSyncBL.AddSyncAccountEvent(this.Account, item.EventGuid, googleCalendarEvent.Id, calendarId);
            CalendarSyncBL.UpdateLogItem(item.EventGuid, googleCalendarEvent.Updated.Value);
        }



    }
}
