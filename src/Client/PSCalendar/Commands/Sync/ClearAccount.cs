using PSCalendarContract;
using PSCalendarTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSCalendar.Commands.Sync
{
    class ClearAccount : BaseCommand<ICalendarSync>
    {
        protected override bool Condition => this.Cmdlet.ClearAccount.NotEmpty();

        public ClearAccount(PSCalendarCmdlet cmdlet) : base(cmdlet) { }

        protected override void Invoke()
        {

            DateTime now, start, end;
            now = start = end = DateTime.Now;
            now = now.AddMonths(this.Cmdlet.ClearAccountMonth);
            start = now.GetFirstMonthDay();
            end = now.GetLastMonthDay();

            InvokeCall(() => this.Client.ClearAccount(this.Cmdlet.ClearAccount, start, end));
        }
    }
}
