using AutoMapper;
using PSCalendarContract.Attributes;
using PSCalendarContract.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using dto = PSCalendarContract.Dto;

namespace PSCalendarServer
{
    public static class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.CreateMap<dto.Event, Event>()
                .ForMember(dst => dst.Type, opt => opt.ResolveUsing<PeriodTypeStringResolver>().FromMember(src => src.Type));
            Mapper.CreateMap<DB.Events, dto.Event>()
                .ForMember(dst => dst.Type, opt => opt.ResolveUsing<PeriodTypeResolver>().FromMember(x => x.Type))
                .ForMember(dst => dst.Color, opt => opt.ResolveUsing<ColorResolver>().FromMember(x => x.Type));

        }

    }

    public class ColorResolver : ValueResolver<string, ConsoleColor>
    {
        protected override ConsoleColor ResolveCore(string source)
        {
            PeriodTypeResolver resolver = new PeriodTypeResolver();
            EventType @event = resolver.ResolvePublic(source);

            FieldInfo fi = @event.GetType().GetField(@event.ToString());

            ColorAttribute[] attributes = (ColorAttribute[])fi.GetCustomAttributes(
                typeof(ColorAttribute),
                false);

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Color;
            }

            return ConsoleColor.White;
        }
    }

    public class PeriodTypeResolver : ValueResolver<string, EventType>
    {
        public EventType ResolvePublic(string source)
        {
            return ResolveCore(source);
        }

        protected override EventType ResolveCore(string source)
        {
            return CommonExtensions.ParseEnum<EventType>(source);
        }
    }

    public class PeriodTypeStringResolver : ValueResolver<EventType, string>
    {
        protected override string ResolveCore(EventType source)
        {
            return source.ToString();
        }
    }
}
