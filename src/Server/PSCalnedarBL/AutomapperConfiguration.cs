using AutoMapper;
using PSCalendarContract.Attributes;
using PSCalendarContract.Dto;
using PSCalendarDB;
using PSCalendarTools;
using System;
using System.Reflection;
using dto = PSCalendarContract.Dto;

namespace PSCalendarBL
{
    public static class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<dto.Event, PSCalendarDB.Event>()
                .ForMember(dst => dst.Type, opt => opt.ResolveUsing<PeriodTypeStringResolver>());
                cfg.CreateMap<PSCalendarDB.Event, dto.Event>()
                .ForMember(dst => dst.Type, opt => opt.ResolveUsing<PeriodTypeResolver>())
                .ForMember(dst => dst.Color, opt => opt.ResolveUsing<ColorResolver>());

                cfg.CreateMap<PSCalendarDB.GoogleCalendarSyncView, dto.GoogleEvent>();

                
            });
        }
    }

    public class ColorResolver : AutoMapper.IValueResolver<PSCalendarDB.Event, dto.Event, int>
    {//todo: maybe move color resolver to gui
        public int Resolve(PSCalendarDB.Event source, dto.Event destination, int destMember, ResolutionContext context)
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

            return 15;
        }
    }
    public class PeriodTypeResolver : IValueResolver<PSCalendarDB.Event, dto.Event, EventType>
    {
        public EventType Resolve(PSCalendarDB.Event source, dto.Event destination, EventType destMember, ResolutionContext context)
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

    public class PeriodTypeStringResolver : IValueResolver<dto.Event, PSCalendarDB.Event, string>
    {
        public string Resolve(dto.Event source, PSCalendarDB.Event destination, string destMember, ResolutionContext context)
        {
            return source.Type.ToString();
        }
    }
}
