using PSCalendarContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSCalendarTools;

namespace PSCalendar.Commands
{
    class SyncAccount : BaseCommand<ICalendarSync>
    {
        protected override bool Condition => this.Cmdlet.SyncAccount.NotEmpty();

        public SyncAccount(PSCalendarCmdlet cmdlet) : base(cmdlet) { }

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

            InvokeCall(() => this.Client.SyncAccount(start, end, this.Cmdlet.SyncAccount));
        }
    }
}
