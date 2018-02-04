CREATE TABLE [gc].[Event](
	[EventGuid] uniqueidentifier not null ,
	[EventId] [int] NOT NULL UNIQUE,
	[Name] [varchar](200) NOT NULL,
	[Date] [datetime] NOT NULL,
	[Type] [varchar](10) NULL,

	CONSTRAINT PK_Event_EventGuid PRIMARY KEY CLUSTERED ([EventGuid]),
	)


