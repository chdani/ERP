USE [Linkia]
GO

/****** Object:  Table [dbo].[PettyCashAccount]    Script Date: 07-Dec-21 12:08:48 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PettyCashAccount](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[AccountCode] [varchar](20) NOT NULL,
	[AccountName] [nvarchar](50) NULL,
	[IsHeadAccount] [bit] NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PettyCashAccount] ADD  CONSTRAINT [DF__PettyCash__IsHea__46486B8E]  DEFAULT ((0)) FOR [IsHeadAccount]
GO


