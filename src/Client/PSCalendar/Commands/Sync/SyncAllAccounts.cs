using PSCalendarContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSCalendarTools;

namespace PSCalendar.Commands
{
    class SyncAllAccounts : BaseCommand<ICalendarSync>
    {
        protected override bool Condition => this.Cmdlet.SyncCurrentMonthAllAccounts.IsPresent || this.Cmdlet.SyncMonth.HasValue;

        public SyncAllAccounts(PSCalendarCmdlet cmdlet) : base(cmdlet) { }

        protected override void Invoke()
        {

            DateTime now, start, end;
            now = start = end = DateTime.Now;
            if (this.Cmdlet.SyncMonth.HasValue)
            {
                now = now.AddMonths(this.Cmdlet.SyncMonth.Value);
            }
            start = now.GetFirstMonthDay();
            end = now.GetLastMonthDay().AddDays(1).Subtract(TimeSpan.FromSeconds(1));

            InvokeCall(() => this.Client.SyncAllAcounts(start, end));
        }
    }
}
