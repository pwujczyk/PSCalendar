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
using System.Net;
using ProductivityTools.MasterConfiguration;

namespace SyncGmailCalendar
{
    public class GoogleCalendarAPI
    {
        static string[] Scopes = { CalendarService.Scope.Calendar };
        public static string EventNotFound = HttpStatusCode.NotFound.ToString();
        //const string CalendarId = "primary";

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

        public Event AddEvent(string account, PSCalendarContract.Dto.Event @event, string calendarId)
        {
            Event e = BuildEvent(@event);
            var request = GetService(account).Events.Insert(e, calendarId);

            Google.Apis.Calendar.v3.Data.Event r = request.Execute();
            return r;

        }

        public Event UpdateEvent(string account, PSCalendarContract.Dto.Event @event, string eventId, string calendarId)
        {
            Event e = BuildEvent(@event);
            var request = GetService(account).Events.Update(e, calendarId, eventId);

            Event r = request.Execute();
            return r;
        }

        public Event GetEvent(string account, string googleEventId, string calendarId)
        {
            Event r = null;
            var request = GetService(account).Events.Get(calendarId, googleEventId);
            try
            {
                r = request.Execute();

            }
            catch (Google.GoogleApiException ex)
            {
                if (ex.HttpStatusCode == HttpStatusCode.NotFound)
                {
                    r = new Event() { Status = EventNotFound };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return r;
        }

        public string Delete(string account, string googleEventId, string calendarId)
        {
            var request = GetService(account).Events.Delete(calendarId, googleEventId);
            string r = request.Execute();
            return r;
        }

        private Event BuildEvent(PSCalendarContract.Dto.Event @event)
        {
            Event result = new Event();
            if (@event.Type == PSCalendarContract.Dto.EventType.Birthday)
            {
                string rule = string.Format($"RRULE:FREQ=YEARLY;BYMONTH={@event.StartDate.Month};BYMONTHDAY={@event.StartDate.Day}");
                result.Recurrence = new List<String>();
                result.Recurrence.Add(rule);
            }

            if (@event.StartDate.Date == @event.StartDate)
            {//whole day event
                result.Start = new EventDateTime() { Date = @event.StartDate.ToString("yyyy-MM-dd") };
                result.End = new EventDateTime() { Date = @event.EndDate.AddDays(1).ToString("yyyy-MM-dd") };
            }
            else
            {
                result.Start = new EventDateTime() { DateTime = @event.StartDate };
                result.End = new EventDateTime() { DateTime = @event.EndDate };
            }


            result.Summary = @event.Name;
            return result;
        }

        private UserCredential Authenticate(string email)
        {
            UserCredential credential;
            string credPath = MConfiguration.Configuration["CredentialPath"];
            var clientSecretPath = Path.Combine(credPath, $".credentials/client_secret.json");
            using (var stream = new FileStream(clientSecretPath, FileMode.Open, FileAccess.Read))
            {
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

        public Events GetGoogleCalendarEvents(string account, DateTime start, DateTime end, string calendarId)
        {
            // Define parameters of request.
            EventsResource.ListRequest request = GetService(account).Events.List(calendarId);
            request.TimeMin = start;
            request.TimeMax = end;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 100;
            request.ShowDeleted = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();
            return events;
        }

        public CalendarList GetGoogleCalendars(string account)
        {
            CalendarListResource.ListRequest request = GetService(account).CalendarList.List();

            CalendarList x = request.Execute();
            return x;
        }

        public Calendar CreateGoogleCalendar(string account, string name)
        {
            Calendar c = new Calendar();
            c.Summary = name;
            var request = GetService(account).Calendars.Insert(c);
            Calendar r = request.Execute();
            return r;
        }
    }
}
