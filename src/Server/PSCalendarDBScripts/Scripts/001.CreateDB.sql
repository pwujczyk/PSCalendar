CREATE TABLE [gc].[Event](
	[EventId] [int] NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[Date] [datetime] NOT NULL,
	[Type] [varchar](10) NULL,
	[Guid] uniqueidentifier not null DEFAULT newid(),
	CONSTRAINT PK_Event_EventID PRIMARY KEY CLUSTERED (EventId),
	)


