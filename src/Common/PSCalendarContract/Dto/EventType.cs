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
        [ColorAttribute(ConsoleColor.Green)]
        Sport = 1,
        [ColorAttribute(ConsoleColor.Magenta)]
        Family = 2,
        [ColorAttribute(ConsoleColor.Yellow)]
        Friends = 3,
        [ColorAttribute(ConsoleColor.Cyan)]
        PawelWork = 4,
        [ColorAttribute(ConsoleColor.DarkCyan)]
        GosiaWork = 5,
        [ColorAttribute(ConsoleColor.DarkGreen)]
        Pawel = 6,
        [ColorAttribute(ConsoleColor.DarkGray)]
        Gosia = 7,
        [ColorAttribute(ConsoleColor.DarkRed)]
        Together = 8



        //ok są jeszcze:
        //DarkCyjan
        //DarkGray
        //DarkGreen
        //DarkRed

    }
}
