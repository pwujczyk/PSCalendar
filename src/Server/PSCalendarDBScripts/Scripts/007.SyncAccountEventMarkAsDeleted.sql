CREATE PROCEDURE [gc].SyncAccountEventMarkAsDeleted
	@GoogleCalendarEventId VARCHAR(40)
AS
BEGIN
	UPDATE sae SET sae.Deleted=1
	FROM [gc].SyncAccountEvent sae 
	WHERE sae.GoogleCalendarEventId=@GoogleCalendarEventId
END