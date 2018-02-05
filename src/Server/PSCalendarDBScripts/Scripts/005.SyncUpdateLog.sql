CREATE TABLE [gc].[SyncAccountLog]
(
	SyncAccountLogId INT IDENTITY(1,1),
	EventGuid UNIQUEIDENTIFIER,
	LastModifcationDate DATETIME NOT NULL,

	CONSTRAINT PK_SyncAccountLog PRIMARY KEY (SyncAccountLogId),
	CONSTRAINT FK_SyncAccountLog_Event FOREIGN KEY (EventGuid) REFERENCES [gc].[Event](EventGuid)
)