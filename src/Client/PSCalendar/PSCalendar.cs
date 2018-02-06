
using PSCalendarContract;
using PSCalendarContract.Dto;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using PSCalendarTools;
using PSCalendar.Commands;

namespace PSCalendar
{
    [Cmdlet(VerbsCommon.Get, "Calendar")]
    public class PSCalendarCmdlet : PSCmdlet
    {
        [Parameter]
        public string Add { get; set; }

        [Parameter]
        public string StartDate { get; set; }

        [Parameter]
        public string EndDate { get; set; }

        [Parameter]
        public string Duration { get; set; }

        [Parameter]
        public EventType Type { get; set; }

        [Parameter]
        public int? ShowMonth { get; set; }

        [Parameter]
        public int? Delete { get; set; }

        [Parameter]
        public int? Change { get; set; }

        [Parameter]
        public SwitchParameter SyncCurrentMonth { get; set; }

        [Parameter]
        public int? SyncMonth { get; set; }

        [Parameter]
        public string AddSyncAccount { get; set; }

        protected override void BeginProcessing()
        {

            List<BaseCommand> conditionTable = new List<BaseCommand>();
            conditionTable.Add(new Delete(this));
            conditionTable.Add(new Add(this));
            conditionTable.Add(new Change(this));
            conditionTable.Add(new Display(this));
            conditionTable.Add(new Sync(this));
            conditionTable.Add(new AddSyncAccount(this));

            foreach (var item in conditionTable)
            {
                item.InvokeCommand();
            }

            base.BeginProcessing();
        } 
    }
}
