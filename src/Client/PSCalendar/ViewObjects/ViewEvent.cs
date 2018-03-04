using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSCalendar.ViewObjects
{
    public class ViewEvent 
    {
        public int NiceId { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int Color { get; set; }


        public ViewEvent(int niceId, string name, DateTime date,int color)
        {
            this.NiceId = niceId;
            this.Name = name;
            this.Date = date;
            this.Color = color;
        }
    }

}
