﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PSCalendarDB
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class CalendarEntities : DbContext
    {
        public CalendarEntities()
            : base("name=CalendarEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<dbUp> dbUp { get; set; }
        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<SyncAccount> SyncAccount { get; set; }
        public virtual DbSet<SyncAccountLog> SyncAccountLog { get; set; }
        public virtual DbSet<SyncAccountEvent> SyncAccountEvent { get; set; }
        public virtual DbSet<GoogleCalendarSyncView> GoogleCalendarSyncView { get; set; }
    
        public virtual int DeleteEventByEventId(Nullable<System.Guid> eventGuid)
        {
            var eventGuidParameter = eventGuid.HasValue ?
                new ObjectParameter("EventGuid", eventGuid) :
                new ObjectParameter("EventGuid", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("DeleteEventByEventId", eventGuidParameter);
        }
    
        public virtual int SyncAccountEventMarkAsDeleted(string googleCalendarEventId)
        {
            var googleCalendarEventIdParameter = googleCalendarEventId != null ?
                new ObjectParameter("GoogleCalendarEventId", googleCalendarEventId) :
                new ObjectParameter("GoogleCalendarEventId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SyncAccountEventMarkAsDeleted", googleCalendarEventIdParameter);
        }
    }
}
