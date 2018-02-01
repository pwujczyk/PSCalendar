using AutoMapper;
using PSCalendarContract.Attributes;
using PSCalendarContract.Dto;
using PSCalendarDB;
using System;
using System.Reflection;
using dto = PSCalendarContract.Dto;

namespace PSCalendarServer
{
    public static class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<dto.Event, PSCalendarDB.Events>()
                .ForMember(dst => dst.Type, opt => opt.ResolveUsing<PeriodTypeStringResolver>());
                cfg.CreateMap<PSCalendarDB.Events, dto.Event>()
                .ForMember(dst => dst.Type, opt => opt.ResolveUsing<PeriodTypeResolver>())
                .ForMember(dst => dst.Color, opt => opt.ResolveUsing<ColorResolver>());
            });
        }
    }

    public class ColorResolver : AutoMapper.IValueResolver<PSCalendarDB.Events, dto.Event, ConsoleColor>
    {
        public ConsoleColor Resolve(PSCalendarDB.Events source, dto.Event destination, ConsoleColor destMember, ResolutionContext context)
        {
            PeriodTypeResolver resolver = new PeriodTypeResolver();
            EventType @event = resolver.ResolvePublic(source.Type);

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
    public class PeriodTypeResolver : IValueResolver<PSCalendarDB.Events, dto.Event, EventType>
    {
        public EventType Resolve(PSCalendarDB.Events source, dto.Event destination, EventType destMember, ResolutionContext context)
        {
            return ResolveCore(source.Type);
        }

        public EventType ResolvePublic(string source)
        {
            return ResolveCore(source);
        }

        protected EventType ResolveCore(string source)
        {
            return CommonExtensions.ParseEnum<EventType>(source);
        }
    }

    public class PeriodTypeStringResolver : IValueResolver<dto.Event, PSCalendarDB.Events, string>
    {
        public string Resolve(Event source, Events destination, string destMember, ResolutionContext context)
        {
            return source.Type.ToString();
        }
    }
}
