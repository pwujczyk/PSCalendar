using PSCalendarContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PSCalendar.Commands
{
    class Delete : BaseCommand<ICalendar>
    {
        public Delete(PSCalendarCmdlet cmdlet) : base(cmdlet) { }

        protected override bool Condition => this.Cmdlet.Delete.HasValue;

        protected override void Invoke()
        {
            DeleteItem(this.Cmdlet.Delete.Value);
        }

        private void DeleteItem(int nullable)
        {
            InvokeCall(() => Client.Delete(nullable));
        }
    }
}
