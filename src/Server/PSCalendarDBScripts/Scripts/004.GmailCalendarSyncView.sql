CREATE VIEW [gc].[GoogleCalendarSyncView] AS
SELECT NiceId,Name,[StartDate],[EndDate],[Type],e.EventGuid,GoogleCalendarId,GoogleCalendarEventId,Email,e.Deleted as EventDeleted, sae.ToBeDeleted as SyncAccountTobeDeleted,sae.Deleted as SyncAccountDeleted FROM [gc].[Event] e
LEFT JOIN [gc].SyncAccountEvent sae ON e.EventGuid=sae.EventGuid
LEFT JOIN [gc].[SyncAccount] sa ON sa.SyncAccountId=sae.SyncAccountId