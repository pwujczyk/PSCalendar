CREATE TABLE [gc].[SyncAccountEvent](
	[SyncAccountEventId] INT NOT NULL IDENTITY(1,1),
	[EventGuid] UniqueIdentifier NOT NULL,
	[GoogleCalendarEventId] VARCHAR(40) NOT NULL,
	[SyncAccountId] INT NOT NULL

	CONSTRAINT PK_SyncAccountEvent PRIMARY KEY ([SyncAccountEventId]),
	CONSTRAINT FK_SyncAccountEvent_Event FOREIGN KEY ([EventGuid]) REFERENCES [gc].[Event]([EventGuid]),
	CONSTRAINT FK_SyncAccountEvent_SyncAccount FOREIGN KEY (SyncAccountID) REFERENCES [gc].SyncAccount([SyncAccountId])
	)


