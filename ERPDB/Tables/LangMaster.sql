USE [Linkia]
GO

/****** Object:  Table [dbo].[LangMaster]    Script Date: 25-Oct-21 7:40:25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LangMaster](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
	[CodeType] [nvarchar](20) NOT NULL,
	[Language] [varchar](10) NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[LangMaster] ADD  CONSTRAINT [DF_LangMaster_id]  DEFAULT (newid()) FOR [Id]
GO


