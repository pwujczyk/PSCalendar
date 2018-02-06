CREATE TABLE [gc].[Event](
	[EventGuid] uniqueidentifier not null ,
	[NiceId] [int] NULL,
	[Name] [varchar](200) NOT NULL,
	[Date] [datetime] NOT NULL,
	[Type] [varchar](10) NULL,
	[Deleted] BIT DEFAULT(0) not null,

	CONSTRAINT PK_Event_EventGuid PRIMARY KEY CLUSTERED ([EventGuid]),

	)


