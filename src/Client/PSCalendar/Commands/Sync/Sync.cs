using PSCalendarContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSCalendar.Commands
{
    class Sync : BaseCommand<ICalendarSync>
    {
        protected override bool Condition => this.Cmdlet.Sync.IsPresent;

        public Sync(PSCalendarCmdlet cmdlet) : base(cmdlet) { }

        protected override void Invoke()
        {
            InvokeCall(() => this.Client.Sync());
        }
    }
}
