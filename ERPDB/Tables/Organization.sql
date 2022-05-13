USE [Linkia]
GO

/****** Object:  Table [dbo].[Organization]    Script Date: 01-May-22 10:08:24 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Organization](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[OrgName] [nvarchar](50) NOT NULL,
	[OrgCode] [varchar](20) NOT NULL,
	[Location] [nvarchar](100) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL,
	[Active] [nvarchar](1) NULL,
	[DefaultOrg] [bit] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Organization] ADD  CONSTRAINT [DF_Organization_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[Organization] ADD  CONSTRAINT [DF_Organization_Active]  DEFAULT (N'Y') FOR [Active]
GO

ALTER TABLE [dbo].[Organization] ADD  DEFAULT ((0)) FOR [DefaultOrg]
GO


