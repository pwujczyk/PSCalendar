using AutoMapper;
using PSCalendarContract.Dto;
using PSCalendarBL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSCalendarDB;

namespace PSCalendarBL
{
    public class CalendarSync : CalendarBase
    {
        public void AddSyncAccount(string email)
        {
            var dbemail = this.Entities.SyncAccount.FirstOrDefault(x => x.Email == email);
            if (dbemail != null)
            {
                throw new Exception($"Account {email} already added to SyncAccount");
            }
            else
            {
                PSCalendarDB.SyncAccount dbSyncAccount = new PSCalendarDB.SyncAccount();
                dbSyncAccount.Email = email;
                this.Entities.SyncAccount.Add(dbSyncAccount);
                this.Entities.SaveChanges();
            }
        }

        public List<string> GetSyncAccounts()
        {
            return this.Entities.SyncAccount.Select(x => x.Email).ToList();
        }

        public void SyncAccountEventMarkAsDeleted(string googleCalendarEventId)
        {
            this.Entities.SyncAccountEventMarkAsDeleted(googleCalendarEventId);
        }

        public List<GoogleEvent> GetSyncEvents(string email, DateTime start, DateTime end)
        {
            //email have to be here as we are using it to check if SyncAccountEvent exists for given account
            List<PSCalendarDB.GoogleCalendarSyncView> googleList = (from i in Entities.GoogleCalendarSyncView.AsNoTracking()
                                                                    where start <= i.StartDate && i.StartDate <= end && i.Email == email
                                                                    select i).ToList();


            List<GoogleEvent> result = Mapper.Map<List<PSCalendarDB.GoogleCalendarSyncView>, List<GoogleEvent>>(googleList);
            return result;
        }

        public void AddSyncAccountEvent(string account, Guid eventGuid, string googleCalendarEventId, string googleCalendarId)
        {
            var syncAccountEvent = new PSCalendarDB.SyncAccountEvent();
            syncAccountEvent.Event = this.Entities.Event.Single(x => x.EventGuid == eventGuid);
            syncAccountEvent.GoogleCalendarEventId = googleCalendarEventId;
            syncAccountEvent.SyncAccount = this.Entities.SyncAccount.Single(x => x.Email == account);
            syncAccountEvent.GoogleCalendarId = googleCalendarId;

            Entities.SyncAccountEvent.Add(syncAccountEvent);
            Entities.SaveChanges();
        }

        public void UpdateGoogleCalendar(string account, Guid eventGuid, EventType eventType, string googleCalendarId)
        {
            var syncAccountEvents = this.Entities.SyncAccountEvent.Single(x => x.EventGuid == eventGuid && x.SyncAccount.Email==account);

            syncAccountEvents.GoogleCalendarId = googleCalendarId;


            var @event = this.Entities.Event.Single(x => x.EventGuid == eventGuid);
            @event.Type = eventType.ToString();
            //var syncAccountEvent = new PSCalendarDB.SyncAccountEvent);
            //syncAccountEvent.Event = this.Entities.Event.Single(x => x.EventGuid == eventGuid);
            //syncAccountEvent.GoogleCalendarEventId = googleCalendarEventId;
            //syncAccountEvent.SyncAccount = this.Entities.SyncAccount.Single(x => x.Email == account);
            //syncAccountEvent.GoogleCalendarId = googleCalendarId;

            //Entities.SyncAccountEvent.Add(syncAccountEvent);
            Entities.SaveChanges();
        }

        //public void GetSyncAccountEvent(string account, DateTime start, DateTime end)
        //{
        //    List<PSCalendarDB.SyncAccountEvent> syncAccountEvents = this.Entities.SyncAccountEvent.Where(x => x.SyncAccount.Email == account).ToList();
        //    List<PSCalendarContract.Dto.SyncAccountEvent> result = Mapper.Map<List<PSCalendarDB.SyncAccountEvent>, List<PSCalendarContract.Dto.SyncAccountEvent>>(syncAccountEvents);
        //}


        public void UpdateLogItem(Guid eventGuid, DateTime datetime)
        {
            var logItem = this.Entities.SyncAccountLog.FirstOrDefault(x => x.EventGuid == eventGuid);
            if (logItem == null)
            {
                logItem = new SyncAccountLog();
                logItem.EventGuid = eventGuid;
                this.Entities.SyncAccountLog.Add(logItem);
            }
            //todo: change to idateprovider
            logItem.LastModifcationDate = datetime;

            Entities.SaveChanges();
        }

        public DateTime GetLastSyncAccountLogItemModyficationDate(Guid eventGuid)
        {
            var syncAccountLogItem = this.Entities.SyncAccountLog.AsNoTracking().Single(x => x.EventGuid == eventGuid);
            return syncAccountLogItem.LastModifcationDate;
        }


        public void DeleteSyncAccountEvent(Guid eventGuid, string account)
        {
            var toDelete=this.Entities.SyncAccountEvent.Single(x => x.EventGuid == eventGuid && x.SyncAccount.Email == account);
            this.Entities.SyncAccountEvent.Remove(toDelete);
            this.Entities.SaveChanges();
        }

        public GoogleEvent GetEvent(string googleId)
        {
            var r = Mapper.Map<PSCalendarDB.GoogleCalendarSyncView, GoogleEvent>(this.Entities.GoogleCalendarSyncView.Single(x => x.GoogleCalendarEventId == googleId));
            return r;
        }

        public void MarkEventAsDeleted(string googleCalendarEventId)
        {
            var syncAccountEvent=this.Entities.SyncAccountEvent.Single(x => x.GoogleCalendarEventId == googleCalendarEventId);

            var allSyncAccountEvents = this.Entities.SyncAccountEvent.Where(x => x.EventGuid == syncAccountEvent.EventGuid);
            foreach (var item in allSyncAccountEvents)
            {
                item.ToBeDeleted = true;
                item.Deleted = true;
            }

            var powershellEvent = this.Entities.Event.Single(x => x.EventGuid == syncAccountEvent.EventGuid);
            powershellEvent.Deleted = true;

            this.Entities.SaveChanges();
            
        }
    }
}
