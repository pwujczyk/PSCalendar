CREATE TABLE [gc].[SyncAccountEvent](
	[SyncAccountId] INT NOT NULL,
	[EventId] INT NOT NULL,

	CONSTRAINT FK_SyncAccountEvent_Event FOREIGN KEY (EventId) REFERENCES [gc].[Event](EventId),
	CONSTRAINT FK_SyncAccountEvent_SyncAccount FOREIGN KEY (SyncAccountID) REFERENCES [gc].SyncAccount([SyncAccountId])
	)


