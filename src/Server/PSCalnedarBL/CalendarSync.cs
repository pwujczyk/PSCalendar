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

        public void SyncAccountEventMarkAsDeleted(string googleCalendarEventId, string email)
        {
            this.Entities.SyncAccountEventMarkAsDeleted(googleCalendarEventId, email);
        }

        //public List<string> Get()
        //{
        //    from sae in Entities.SyncAccountEvent 
        //    join sa in Entities.SyncAccount on sae.SyncAccountId.Equals(sae.SyncAccountId)
        //    select new 

        //}

        public List<GoogleEvent> GetSyncEvents(string email, DateTime start, DateTime end)
        {
            List<PSCalendarDB.GoogleCalendarSyncView> googleList = (from i in Entities.GoogleCalendarSyncView.AsNoTracking()
                                                                    where start <= i.StartDate && i.StartDate <= end// && i.Email == email
                                                                    select i).ToList();


            List<GoogleEvent> result = Mapper.Map<List<PSCalendarDB.GoogleCalendarSyncView>, List<GoogleEvent>>(googleList);
            return result;
        }

        public void UpdateSyncAccountEvent(string account, Guid eventGuid, string googleCalendarEventId)
        {
            var syncAccountEvent = new PSCalendarDB.SyncAccountEvent();
            syncAccountEvent.Event = this.Entities.Event.Single(x => x.EventGuid == eventGuid);
            syncAccountEvent.GoogleCalendarEventId = googleCalendarEventId;
            syncAccountEvent.SyncAccount = this.Entities.SyncAccount.Single(x => x.Email == account);

            Entities.SyncAccountEvent.Add(syncAccountEvent);
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
            var syncAccountLogItem = this.Entities.SyncAccountLog.Single(x => x.EventGuid == eventGuid);
            return syncAccountLogItem.LastModifcationDate;
        }


        public GoogleEvent GetEvent(string googleId)
        {
            return Mapper.Map<PSCalendarDB.GoogleCalendarSyncView, GoogleEvent>(this.Entities.GoogleCalendarSyncView.Single(x => x.GoogleCalendarEventId == googleId));
        }
    }
}
