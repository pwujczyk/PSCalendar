using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSCalendar
{
    class ConditionTable
    {
        public Func<bool> Condition { get; set; }
        public Action Action { get; set; }

        public ConditionTable(Func<bool> condition, Action action)
        {
            this.Condition = condition;
            this.Action = action;
        }
    }
}
