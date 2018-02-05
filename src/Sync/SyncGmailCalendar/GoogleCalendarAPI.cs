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
    public class GoogleCalendarAPI
    {
        static string[] Scopes = { CalendarService.Scope.Calendar };
        const string CalendarId = "primary";
        
        private CalendarService GetService(string account)
        {
            var credential = Authenticate(account);

            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                //ApplicationName = ApplicationName,
            });
            return service;
        }

        public Event AddEvent(string account, PSCalendarContract.Dto.Event @event)
        {
            Event e = BuildEvent(@event);
            var request = GetService(account).Events.Insert(e, CalendarId);

            Google.Apis.Calendar.v3.Data.Event r = request.Execute();
            return r;

        }

        public void UpdateEvent(string account, PSCalendarContract.Dto.Event @event, string eventId)
        {
            Event e = BuildEvent(@event);
            var request = GetService(account).Events.Update(e, CalendarId,eventId);

           Event r = request.Execute();
        }

        public Event GetEvent(string account, string googleEventId)
        {
            var request = GetService(account).Events.Get(CalendarId, googleEventId);
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

        public Events GetGoogleCalendarEvents(string account, DateTime start, DateTime end)
        {
            // Define parameters of request.
            EventsResource.ListRequest request = GetService(account).Events.List("primary");
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
