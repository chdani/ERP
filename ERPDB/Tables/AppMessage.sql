USE [Linkia]
GO

/****** Object:  Table [dbo].[AppMessage]    Script Date: 10-Mar-22 2:10:20 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AppMessage](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Code] [varchar](20) NOT NULL,
	[Description] [nvarchar](2000) NOT NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL,
	 CONSTRAINT [UK_Appmessage_Code] UNIQUE NONCLUSTERED 
	(
		[Code] ASC
	) 
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[AppMessage] ADD  CONSTRAINT [DF_AppMessage_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[AppMessage] ADD  CONSTRAINT [DF_AppMessage_Active]  DEFAULT (N'Y') FOR [Active]
GO


