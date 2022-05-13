USE [Linkia]
GO

/****** Object:  Table [dbo].[JobPosition]    Script Date: 19-Jan-22 10:27:46 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[JobPosition](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Code] [varchar](10) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[JobPosition] ADD  CONSTRAINT [DF_JobPosition_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[JobPosition] ADD  CONSTRAINT [DF_JobPosition_Active]  DEFAULT (N'Y') FOR [Active]
GO


