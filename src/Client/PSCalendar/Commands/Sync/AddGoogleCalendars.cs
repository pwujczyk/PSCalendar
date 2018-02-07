using PSCalendarContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSCalendar.Commands.Core
{
    class AddGoogleCalendars : BaseCommand<ICalendarSync>
    {
        public AddGoogleCalendars(PSCalendarCmdlet cmdl) : base(cmdl) { }

        protected override bool Condition => this.Cmdlet.AddGoogleCalendars;

        protected override void Invoke()
        {
            InvokeCall(() => Client.AddCalendars());
        }
    }
}
