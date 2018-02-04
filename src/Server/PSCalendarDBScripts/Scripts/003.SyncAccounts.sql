CREATE TABLE [gc].[SyncAccountEvent](
	[SyncAccountId] INT NOT NULL,
	[EventGuid] UniqueIdentifier NOT NULL,
	[GoogleCalendarId] VARCHAR(40) NOT NULL,

	CONSTRAINT PK_SyncAccountEvent PRIMARY KEY ([SyncAccountId]),
	CONSTRAINT FK_SyncAccountEvent_Event FOREIGN KEY ([EventGuid]) REFERENCES [gc].[Event]([EventGuid]),
	CONSTRAINT FK_SyncAccountEvent_SyncAccount FOREIGN KEY (SyncAccountID) REFERENCES [gc].SyncAccount([SyncAccountId])
	)


