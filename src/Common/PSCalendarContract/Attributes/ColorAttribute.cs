using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSCalendarContract.Attributes
{
    public class ColorAttribute : Attribute
    {
        //public ConsoleColor Color { get; set; }
        public int Color { get; set; }
        public ColorAttribute(int color)
        {
            this.Color = color;
        }
    }
}
