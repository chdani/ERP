USE [Linkia]
GO

/****** Object:  Table [dbo].[AppAccess]    Script Date: 11-Oct-21 9:22:09 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AppAccess](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[AccessName] [varchar](50) NOT NULL,
	[AccessCode] [varchar](20) NOT NULL,
	[AccessType] [varchar](20) NOT NULL,
	[ScreenUrl] [varchar](200) NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[AppAccess] ADD  CONSTRAINT [DF_AppAccess_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[AppAccess] ADD  CONSTRAINT [DF_AppAccess_Active]  DEFAULT (N'Y') FOR [Active]
GO


