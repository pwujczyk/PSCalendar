CREATE TABLE [gc].[Events](
	[EventsId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[Date] [datetime] NOT NULL,
	[Type] [varchar](10) NULL,
	[NiceId] [int] NULL,
	CONSTRAINT PK_Events_EventsID PRIMARY KEY CLUSTERED (EventsId)
	)


