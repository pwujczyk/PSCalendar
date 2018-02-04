CREATE VIEW [gc].[GoogleCalendarSyncView] AS
SELECT EventId,Name,[Date],[Type],e.EventGuid,sae.SyncAccountId,GoogleCalendarId,Email FROM [gc].[Event] e
LEFT JOIN [gc].SyncAccountEvent sae ON e.EventGuid=sae.EventGuid
LEFT JOIN [gc].[SyncAccount] sa ON sa.SyncAccountId=sae.SyncAccountId