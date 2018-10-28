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
                return this.Cmdlet.Change.HasValue && (this.Cmdlet.Add.NotEmpty() || this.Cmdlet.StartDate.NotEmpty() || this.Cmdlet.Type != EventType.None);
            }
        }


        public Change(PSCalendarCmdlet cmdlet) : base(cmdlet) { }

        protected override void Invoke()
        {
            ChangeItem();
        }

        private void ChangeItem()
        {
            DateTime startDate = this.Cmdlet.StartDate == null ? DateTime.MinValue : DateTime.Parse(this.Cmdlet.StartDate);

            DateTime endDate = DateTime.MinValue;
            string endDateStr = this.Cmdlet.EndDate;
            string durationStr = this.Cmdlet.Duration;
            //if (endDateStr.NotEmpty() || durationStr.NotEmpty())
            //{
            //    endDate = EndDateTools.GetEndDate(startDate, endDateStr, durationStr);
            //}
           // if (startDate!=DateTime.MinValue && endDate==DateTime.MinValue)
            {
                endDate= EndDateTools.GetEndDate(startDate, endDateStr, durationStr);
            }


            var @event = new Event { NiceId = this.Cmdlet.Change.Value, StartDate = startDate, EndDate = endDate, Name = this.Cmdlet.Add, Type = this.Cmdlet.Type };
            InvokeCall(() => Client.ChangeEvent(@event));
        }
    }
}
