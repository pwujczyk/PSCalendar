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

namespace SyncGmailCalendar
{
    public class SyncGoogleCalendar
    {
        static string[] Scopes = { CalendarService.Scope.Calendar };

        public void Sync()
        {
            var credentials = Authenticate("pwujczyk@gmail.com");
            var events = GetAllEvents(credentials);

            DisplayEvents(events);
            AddEvent(credentials);
            DisplayEvents(events);
        }

        private void AddEvent(UserCredential credential)
        {
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                //ApplicationName = ApplicationName,
            });

            Event e = new Event();

            e.Start = new EventDateTime() { DateTime = DateTime.Now };
            e.End = new EventDateTime() { DateTime = DateTime.Now.AddHours(1) };
            e.Description = "description";
            e.Summary = "summary";

            var request = service.Events.Insert(e, "Accenture");
            var r = request.Execute();

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

        private Events GetAllEvents(UserCredential credential)
        {
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                //ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = DateTime.Now.AddMonths(-1);
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();


            return events;
        }
    }
}
