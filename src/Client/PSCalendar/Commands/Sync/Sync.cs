using PSCalendarContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSCalendarTools;

namespace PSCalendar.Commands
{
    class Sync : BaseCommand<ICalendarSync>
    {
        protected override bool Condition => this.Cmdlet.SyncCurrentMonth.IsPresent || this.Cmdlet.SyncMonth.HasValue;

        public Sync(PSCalendarCmdlet cmdlet) : base(cmdlet) { }

        protected override void Invoke()
        {

            DateTime now, start, end;
            now = start = end = DateTime.Now;
            if (this.Cmdlet.SyncMonth.HasValue)
            {
                now = now.AddMonths(this.Cmdlet.SyncMonth.Value);
            }
            start = now.GetFirstMonthDay();
            end = now.GetLastMonthDay();

            InvokeCall(() => this.Client.Sync(start, end));
        }
    }
}
