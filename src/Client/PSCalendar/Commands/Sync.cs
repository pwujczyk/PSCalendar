using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSCalendar.Commands
{
    class Sync : BaseCommand
    {
        protected override bool Condition => throw new NotImplementedException();

        public Sync(PSCalendarCmdlet cmdlet) : base(cmdlet) { }

        protected override void Invoke()
        {
            InvokeCall(() => this.Client.Sync());
        }
    }
}
