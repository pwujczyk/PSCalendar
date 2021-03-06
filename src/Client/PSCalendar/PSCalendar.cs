﻿
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
using PSCalendar.Commands.Core;
using PSCalendar.Commands.Sync;
using System.Runtime.InteropServices;

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
        public SwitchParameter SyncCurrentMonthAllAccounts { get; set; }

        [Parameter]
        public int? SyncMonth { get; set; }

        [Parameter]
        public string ClearAccount{ get; set; }

        [Parameter]
        public int ClearAccountMonth { get; set; }

        [Parameter]
        public string SyncAccount { get; set; }

        [Parameter]
        public string AddSyncAccount { get; set; }

        [Parameter]
        public string AddGoogleCalendarsToAccount { get; set; }

        //Colors
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(IntPtr handle, out int mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int handle);
        //Colors
        protected override void BeginProcessing()
        {

            var handle = GetStdHandle(-11);
            int mode;
            GetConsoleMode(handle, out mode);
            SetConsoleMode(handle, mode | 0x4);

            List<BaseCommand> conditionTable = new List<BaseCommand>();
            conditionTable.Add(new Delete(this));
            conditionTable.Add(new Add(this));
            conditionTable.Add(new Change(this));
            conditionTable.Add(new Display(this));
            conditionTable.Add(new SyncAccount(this));
            conditionTable.Add(new SyncAllAccounts(this));
            conditionTable.Add(new AddSyncAccount(this));
            conditionTable.Add(new AddGoogleCalendars(this));
            conditionTable.Add(new ClearAccount(this));

            foreach (var item in conditionTable)
            {
                item.InvokeCommand();
            }

            base.BeginProcessing();
        } 
    }
}
