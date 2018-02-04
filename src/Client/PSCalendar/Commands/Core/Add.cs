using PSCalendarContract.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSCalendarTools;
using PSCalendarContract;

namespace PSCalendar.Commands
{
    class Add : BaseCommand<ICalendar>
    {
        public Add(PSCalendarCmdlet cmdl) : base(cmdl) { }

        protected override bool Condition
        {
            get
            {
                return this.Cmdlet.Add.NotEmpty() && this.Cmdlet.Date.NotEmpty() && this.Cmdlet.Type != EventType.None;
            }
        }

        protected override void Invoke()
        {
            AddNewEvent(this.Cmdlet.Add, DateTime.Parse(this.Cmdlet.Date), this.Cmdlet.Type);
        }

        private void AddNewEvent(string name, DateTime date, EventType type)
        {
            InvokeCall(() => Client.AddEvent(new Event { Date = date, Name = name, Type = type }));
        }
    }
}
