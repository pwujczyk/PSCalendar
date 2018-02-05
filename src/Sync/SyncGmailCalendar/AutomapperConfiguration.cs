using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncGmailCalendar
{
    public static class AutomapperConfiguration
    {

        public static MapperConfiguration dtoConfig;

        public static void Configure()
        {
            dtoConfig=new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Google.Apis.Calendar.v3.Data.Event, PSCalendarContract.Dto.Event>()
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Summary))
                .ForMember(dst => dst.Date, opt => opt.MapFrom(src => src.Start.DateTime))
                .ForMember(dst => dst.EventGuid, opt => opt.Ignore())
                .ForMember(dst => dst.EventsId, opt => opt.Ignore())
                .ForMember(dst => dst.Type, opt => opt.Ignore());
            });
        }
    }
}
