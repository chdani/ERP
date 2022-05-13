USE [Linkia]
GO

/****** Object:  Table [dbo].[CodesDetail]    Script Date: 25-Oct-21 7:40:12 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CodesDetail](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL,
	[CodesMasterId] [uniqueidentifier] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CodesDetail] ADD  CONSTRAINT [DF_CodesDetail_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[CodesDetail] ADD  CONSTRAINT [DF_CodesDetail_Active]  DEFAULT (N'Y') FOR [Active]
GO


