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
                return this.Cmdlet.Add.NotEmpty() && this.Cmdlet.StartDate.NotEmpty() && this.Cmdlet.Type != EventType.None;
            }
        }

        protected override void Invoke()
        {
            DateTime startDate = DateTime.Parse(this.Cmdlet.StartDate);
            DateTime endDate = EndDateTools.GetEndDate(startDate, this.Cmdlet.EndDate, this.Cmdlet.Duration);
            AddNewEvent(this.Cmdlet.Add, startDate, endDate, this.Cmdlet.Type);
        }

        private void AddNewEvent(string name, DateTime startDate, DateTime endDate, EventType type)
        {
            InvokeCall(() => Client.AddEvent(new Event { StartDate = startDate, EndDate = endDate, Name = name, Type = type }));
        }
    }
}
