CREATE PROCEDURE [gc].DeleteEventByEventId
	@EventGuid UNIQUEIDENTIFIER
AS
	SET NOCOUNT ON
	
	DECLARE @TranName VARCHAR(20);
	SELECT @TranName = 'DeleteEventByEventIdTran';

	BEGIN TRANSACTION @TranName
	UPDATE sae SET  sae.ToBeDeleted=1
	FROM [gc].[Event] e
	LEFT JOIN [gc].SyncAccountEvent sae ON e.EventGuid=sae.EventGuid
	WHERE e.EventGuid=@EventGuid

	UPDATE [gc].[Event] SET Deleted=1,NiceId=NULL WHERE EventGuid=@EventGuid
	COMMIT TRANSACTION @TranName