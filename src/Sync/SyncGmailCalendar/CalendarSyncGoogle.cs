using Google.Apis.Calendar.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSCalendarTools;
using PSCalendarSyncGoogle;
using PSCalendarSyncGoogle.Syncs;

namespace SyncGmailCalendar
{
    public class CalendarSyncGoogle
    {
        GoogleCalendarAPI SyncGoogleCalendarAPI = new GoogleCalendarAPI();
      //  PSCalendarBL.CalendarCore CalendarCoreBL = new PSCalendarBL.CalendarCore();
        PSCalendarBL.CalendarSync CalendarSyncBL = new PSCalendarBL.CalendarSync();
        //Dictionary<PSCalendarContract.Dto.EventType, string> CalendarList;

        static CalendarSyncGoogle()
        {
            AutomapperConfiguration.Configure();

        }

        public CalendarSyncGoogle()
        {

        }

        public void SyncAllAcounts(DateTime start, DateTime end)
        {
            var accounts = CalendarSyncBL.GetSyncAccounts();
            foreach (var account in accounts)
            {
                Dictionary<PSCalendarContract.Dto.EventType, string> calendarList= ValidadteCalendarList(account);
                new GoogleToPSSync(account, start, end, calendarList).Sync();
            }

            foreach (var account in accounts)
            {
                Dictionary<PSCalendarContract.Dto.EventType, string> calendarList = ValidadteCalendarList(account);
                new PSToGoogleSync(account, start, end, calendarList).Sync();
            }
        }

        public void ClearAccount(string account, DateTime start, DateTime end)
        {
            Dictionary<PSCalendarContract.Dto.EventType, string> calendarList = ValidadteCalendarList(account);
            foreach (var calendar in calendarList)
            {
                Events events = this.SyncGoogleCalendarAPI.GetGoogleCalendarEvents(account, start, end, calendar.Value);
                foreach (var @event in events.Items)
                {
                    if (@event.Status != "cancelled")
                    {
                        this.SyncGoogleCalendarAPI.Delete(account, @event.Id, calendar.Value);
                    }
                }
            }

            var syncEvents=this.CalendarSyncBL.GetSyncEvents(account, start, end);
            foreach (var syncEvent in syncEvents)
            {
                CalendarSyncBL.DeleteSyncAccountEvent(syncEvent.EventGuid, account);
            }
            
        }

        public void SyncAccount(string account, DateTime start, DateTime end)
        {
            Dictionary<PSCalendarContract.Dto.EventType, string> calendarList = ValidadteCalendarList(account);
            new GoogleToPSSync(account, start, end, calendarList).Sync();
            new PSToGoogleSync(account, start, end, calendarList).Sync();
        }

        public void CreateCalendars(string account)
        {
            IEnumerable<string> calendars = SyncGoogleCalendarAPI.GetGoogleCalendars(account).Items.Select(x => x.Summary).ToList();
            List<string> categories = GetEventTypesAsStringList();
            foreach (var item in categories.Except(calendars))
            {
                if (item == PSCalendarContract.Dto.EventType.None.ToString()) continue;
                SyncGoogleCalendarAPI.CreateGoogleCalendar(account, item);
            }
        }

        private static List<string> GetEventTypesAsStringList()
        {
            return Enum.GetNames(typeof(PSCalendarContract.Dto.EventType))
                .Except(new List<string>() { PSCalendarContract.Dto.EventType.None.ToString() }).ToList();
        }

        private Dictionary<PSCalendarContract.Dto.EventType, string> ValidadteCalendarList(string account)
        {
            var calendarList = SyncGoogleCalendarAPI.GetGoogleCalendars(account).Items;
            List<string> psCategories = GetEventTypesAsStringList();
            List<string> googleCalendarTypes = calendarList.Select(i => i.Summary).ToList();
            var allCalendarExists = psCategories.Where(x => !googleCalendarTypes.Contains(x));//.Where(x => !categories.Contains(x));
            if (allCalendarExists.Count() == 0)
            {
                Dictionary<PSCalendarContract.Dto.EventType, string> result = new Dictionary<PSCalendarContract.Dto.EventType, string>();
                foreach (var calendarItem in calendarList)
                {
                    var key = calendarItem.Summary.TryParse<PSCalendarContract.Dto.EventType>();
                    if (key != PSCalendarContract.Dto.EventType.None)
                    {
                        result.Add(key, calendarItem.Id);
                    }
                }
                return result;
            }
            else
            {
                throw new Exception($"Not all calendar exists in the google callendar, check it out {account} {allCalendarExists.ToFlatString()}");
            }
        }

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
