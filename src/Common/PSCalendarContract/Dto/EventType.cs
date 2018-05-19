using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;
using PSCalendarContract.Attributes;

namespace PSCalendarContract.Dto
{
    public enum EventType
    {
        //None = 0,
        //[ColorAttribute(ConsoleColor.Green)]
        //Sport = 1,
        //[ColorAttribute(ConsoleColor.Magenta)]
        //Family = 2,
        //[ColorAttribute(ConsoleColor.Yellow)]
        //Friends = 3,
        //[ColorAttribute(ConsoleColor.Cyan)]
        //Accenture = 4,
        //[ColorAttribute(ConsoleColor.DarkCyan)]
        //BRE = 5,
        //[ColorAttribute(ConsoleColor.DarkGreen)]
        //PawelPC=6,
        //[ColorAttribute(ConsoleColor.DarkGray)]
        //Gosia=7


        None = 0,
        [ColorAttribute(46)]//green
        Sport = 1,
        [ColorAttribute(200)]
        Family = 2,
        [ColorAttribute(226)]
        Friends = 3,
        [ColorAttribute(168)]
        PawelWork = 4,
        [ColorAttribute(156)]
        GosiaWork = 5,
        [ColorAttribute(106)]
        Pawel = 6,
        [ColorAttribute(240)]
        Gosia = 7,
        [ColorAttribute(214)]
        Together = 8



        //ok są jeszcze:
        //DarkCyjan
        //DarkGray
        //DarkGreen
        //DarkRed

    }
}
