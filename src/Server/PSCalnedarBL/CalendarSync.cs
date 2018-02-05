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


        public List<GoogleEvent> GetSyncEvents(DateTime start, DateTime end)
        {
            List<PSCalendarDB.GoogleCalendarSyncView> googleList = (from i in Entities.GoogleCalendarSyncView
                                                                    where start <= i.Date && i.Date <= end
                                                                select i).ToList();


            List<GoogleEvent> result = Mapper.Map<List<PSCalendarDB.GoogleCalendarSyncView>, List<GoogleEvent>>(googleList);
            return result;
        }

        public void UpdateSyncAccountEvent(string account,PSCalendarContract.Dto.Event @event, string googleCalendarEventId)
        {
            SyncAccountEvent syncAccountEvent = new SyncAccountEvent();
            syncAccountEvent.Event = this.Entities.Event.Single(x => x.EventGuid == @event.EventGuid);
            syncAccountEvent.GoogleCalendarEventId = googleCalendarEventId;
            syncAccountEvent.SyncAccount = this.Entities.SyncAccount.Single(x => x.Email == account);

            Entities.SyncAccountEvent.Add(syncAccountEvent);
            Entities.SaveChanges();
        }

        public void UpdateLogItem(Guid eventGuid)
        {
            UpdateLogItem(eventGuid, DateTime.Now);
        }

        public void UpdateLogItem(Guid eventGuid,DateTime datetime)
        {
            var logItem=this.Entities.SyncAccountLog.FirstOrDefault(x => x.EventGuid == eventGuid);
            if (logItem==null)
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
            var syncAccountLogItem=this.Entities.SyncAccountLog.Single(x => x.EventGuid == eventGuid);
            return syncAccountLogItem.LastModifcationDate;
        }
    }
}
