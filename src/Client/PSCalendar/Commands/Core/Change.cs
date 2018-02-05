using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSCalendarContract;
using PSCalendarContract.Dto;
using PSCalendarTools;

namespace PSCalendar.Commands
{
    class Change : BaseCommand<ICalendar>
    {


        protected override bool Condition
        {
            get
            {
                return this.Cmdlet.Change.HasValue && (this.Cmdlet.Add.NotEmpty() || this.Cmdlet.Date.NotEmpty() || this.Cmdlet.Type != EventType.None);
            }
        }


        public Change(PSCalendarCmdlet cmdlet) : base(cmdlet) { }

        protected override void Invoke()
        {
            ChangeItem();
        }

        private void ChangeItem()
        {
            DateTime dt = this.Cmdlet.Date == null ? DateTime.MinValue : DateTime.Parse(this.Cmdlet.Date);
            var @event = new Event { EventId = this.Cmdlet.Change.Value, Date = dt, Name = this.Cmdlet.Add, Type = this.Cmdlet.Type };
            InvokeCall(() => Client.ChangeEvent(@event));
        }
    }
}
