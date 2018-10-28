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
        Together = 8,

        [ColorAttribute(123)]
        Birthday =9



        //ok są jeszcze:
        //DarkCyjan
        //DarkGray
        //DarkGreen
        //DarkRed

    }
}
