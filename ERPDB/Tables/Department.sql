USE [Linkia]
GO

/****** Object:  Table [dbo].[Department]    Script Date: 23-Mar-22 10:00:36 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Department](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Code] [varchar](20) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[ParentId] [uniqueidentifier] NULL,
	[Type] [varchar](50) NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Department] ADD  CONSTRAINT [DF_Department_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[Department] ADD  CONSTRAINT [DF_Department_Active]  DEFAULT (N'Y') FOR [Active]
GO


