CREATE VIEW [gc].[GoogleCalendarSyncView] AS
with cross1 as(
select DISTINCT e.NiceId,e.Name,e.StartDate,e.EndDate,e.Type,e.EventGuid,e.Deleted
, sae.SyncAccountId 
from  [gc].[Event] e
cross join [gc].[SyncAccountEvent] sae
)
select c.NiceId,c.Name,c.StartDate,c.EndDate,c.[Type],c.EventGuid,sae.GoogleCalendarEventId,c.Deleted as EventDeleted,
sae.ToBeDeleted as SyncAccountTobeDeleted,sae.Deleted as SyncAccountDeleted,
sa.email
from cross1 c
left join [gc].[SyncAccountEvent] sae on c.EventGuid=sae.EventGuid  and c.SyncAccountId=sae.SyncAccountId
left join  [gc].[SyncAccount] sa ON sa.SyncAccountId=sae.SyncAccountId