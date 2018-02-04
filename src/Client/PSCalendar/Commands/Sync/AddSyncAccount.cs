using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSCalendarContract;
using PSCalendarTools;

namespace PSCalendar.Commands
{
    class AddSyncAccount : BaseCommand<ICalendarSync>
    {
        protected override bool Condition => this.Cmdlet.AddSyncAccount.NotEmpty();

        public AddSyncAccount(PSCalendarCmdlet cmdlet) : base(cmdlet) { }

        protected override void Invoke()
        {
            this.Client.AddSyncAccount(this.Cmdlet.AddSyncAccount);
        }
    }
}
