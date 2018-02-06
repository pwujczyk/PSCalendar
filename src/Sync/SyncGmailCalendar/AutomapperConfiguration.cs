using AutoMapper;
using Google.Apis.Calendar.v3.Data;
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
            dtoConfig = new MapperConfiguration(cfg =>
              {
                  cfg.CreateMap<Google.Apis.Calendar.v3.Data.Event, PSCalendarContract.Dto.Event>()
                  .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Summary))
                  .ForMember(dst => dst.Date, opt => opt.ResolveUsing<DateResolver, EventDateTime>(x => x.Start))// .MapFrom(src => src.Start.DateTime))
                  .ForMember(dst => dst.EventGuid, opt => opt.Ignore())
                  .ForMember(dst => dst.NiceId, opt => opt.Ignore())
                  .ForMember(dst => dst.Type, opt => opt.Ignore());
              });
        }
    }

    public class DateResolver : IMemberValueResolver<object, object, EventDateTime, DateTime>
    {
        public DateTime Resolve(object source, object destination, EventDateTime sourceMember, DateTime destMember, ResolutionContext context)
        {
            if (sourceMember.DateTime.HasValue)
            {
                return sourceMember.DateTime.Value;
            }
            else
            {
                return DateTime.Parse(sourceMember.Date);
            }
        }
    }
}
