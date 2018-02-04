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

        public void UpdateEventWithGoogleId(string account,PSCalendarContract.Dto.Event @event, string googleId)
        {
            SyncAccountEvent syncAccountEvent = new SyncAccountEvent();
            syncAccountEvent.Event = this.Entities.Event.Single(x => x.EventGuid == @event.EventGuid);
            syncAccountEvent.GoogleCalendarId = googleId;
            syncAccountEvent.SyncAccount = this.Entities.SyncAccount.Single(x => x.Email == account);

            Entities.SyncAccountEvent.Add(syncAccountEvent);
            Entities.SaveChanges();
        }
    }
}
