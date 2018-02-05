using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PSCalendarTools;
using AutoMapper;

namespace SyncGmailCalendar
{
    public class SyncGoogleCalendar
    {
        static string[] Scopes = { CalendarService.Scope.Calendar };
         const string CalendarId = "primary";

        static SyncGoogleCalendar()
        {
            AutomapperConfiguration.Configure();
        }

        public void Sync(string account)
        {
            //GetGoogleCalendarEventsAndDisplay(account);

            var psCalendar = new PSCalendarBL.CalendarCore();
            var start = DateTime.Now.GetFirstMonthDay();
            var end = DateTime.Now.GetLastMonthDay();
            var psEvents = psCalendar.GetEvents(start, end);
            var xxx = new PSCalendarBL.CalendarSync();
            var psEventsForGoogle = xxx.GetSyncEvents(start, end);
            var googleEvents = GetGoogleCalendarEvents(account, start, end);
            GetGoogleCalendarEventsAndDisplay(account, start, end);

            foreach (var item in psEventsForGoogle)
            {
                if (item.GoogleCalendarEventId == null)
                {
                    var googleCalendarevent = AddEvent(account, item);
                    UpdateSyncAccountEvent(account, item, googleCalendarevent.Id);
                    xxx.UpdateLogItem(item.EventGuid);
                }
                else
                {
                    var googleCalendarEvent = GetEvent(account, item.GoogleCalendarEventId);
                    var lastSyncAccountLogItemModyficationDate = xxx.GetLastSyncAccountLogItemModyficationDate(item.EventGuid);
                    if (googleCalendarEvent.Updated > lastSyncAccountLogItemModyficationDate)
                    {
                        PSCalendarContract.Dto.Event @event = new PSCalendarContract.Dto.Event();
                        var mapper = AutomapperConfiguration.dtoConfig.CreateMapper();
                        @event = mapper.Map<Google.Apis.Calendar.v3.Data.Event, PSCalendarContract.Dto.Event>(googleCalendarEvent);
                        psCalendar.ChangeEvent(@event);
                        xxx.UpdateLogItem(item.EventGuid, googleCalendarEvent.Updated.Value);
                    }

                    if (googleCalendarEvent.Updated < lastSyncAccountLogItemModyficationDate)
                    {
                        UpdateEvent(account, item, googleCalendarEvent.Id);
                        xxx.UpdateLogItem(item.EventGuid, googleCalendarEvent.Updated.Value);
                    }

                } 
            }

            //   AddEvent(account);
            GetGoogleCalendarEventsAndDisplay(account, start, end);
        }

        private void UpdateSyncAccountEvent(string acccount, PSCalendarContract.Dto.GoogleEvent item, string googleCalendarEventId)
        {
            var xxx = new PSCalendarBL.CalendarSync();
            xxx.UpdateSyncAccountEvent(acccount, item, googleCalendarEventId);
        }

        private void GetGoogleCalendarEventsAndDisplay(string account, DateTime start, DateTime end)
        {
            var events = GetGoogleCalendarEvents(account, start, end);
            DisplayEvents(events);
        }

        private Event AddEvent(string account, PSCalendarContract.Dto.Event @event)
        {
            var credential = Authenticate(account);

            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                //ApplicationName = ApplicationName,
            });

            Event e = BuildEvent(@event);

            
            var request = service.Events.Insert(e, CalendarId);

            Google.Apis.Calendar.v3.Data.Event r = request.Execute();
            return r;

        }

        private void UpdateEvent(string account, PSCalendarContract.Dto.Event @event, string eventId)
        {
            var credential = Authenticate(account);

            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                //ApplicationName = ApplicationName,
            });

            Event e = BuildEvent(@event);


            var request = service.Events.Update(e, CalendarId,eventId);

            Google.Apis.Calendar.v3.Data.Event r = request.Execute();
            //return r;
        }

        private Event GetEvent(string account, string googleEventId)
        {
            var credential = Authenticate(account);

            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                //ApplicationName = ApplicationName,
            });


            var request = service.Events.Get(CalendarId, googleEventId);
            Event r = request.Execute();
            return r;
        }


        private Event BuildEvent(PSCalendarContract.Dto.Event @event)
        {
            Event result = new Event();
            if (@event.Date.Date == @event.Date)
            {//whole day event
                result.Start = new EventDateTime() { Date = @event.Date.ToString("yyyy-MM-dd") };
                result.End = new EventDateTime() { Date = @event.Date.ToString("yyyy-MM-dd") };
            }
            else
            {
                result.Start = new EventDateTime() { DateTime = @event.Date };
                result.End = new EventDateTime() { DateTime = @event.Date.AddHours(1) };
            }
            result.Summary = @event.Name;
            return result;
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
            //  Console.Read();
        }

        private UserCredential Authenticate(string email)
        {
            UserCredential credential;

            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, $".credentials/{email}.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }
            return credential;
        }

        private Events GetGoogleCalendarEvents(string account, DateTime start, DateTime end)
        {
            UserCredential credential = Authenticate(account);
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                //ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = start;
            request.TimeMax = end;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 100;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();


            return events;
        }
    }
}
