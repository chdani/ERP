USE [Linkia]
GO

/****** Object:  Table [dbo].[PettyCashTransfer]    Script Date: 23-Dec-21 10:30:56 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PettyCashTransfer](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[FinYear] [varchar](20) NOT NULL,
	[FromTellerId] [uniqueidentifier] NOT NULL,
	[FromAccountId] [uniqueidentifier] NOT NULL,
	[FromOrgId] [uniqueidentifier] NOT NULL,
	[ToTellerId] [uniqueidentifier] NOT NULL,
	[ToAccountId] [uniqueidentifier] NOT NULL,
	[ToOrgId] [uniqueidentifier] NOT NULL,
	[TransDate] [date] NOT NULL,
	[Amount] [numeric](20, 6) NOT NULL,
	[Remarks] [nvarchar](300) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO


ALTER TABLE [dbo].[PettyCashTransfer] ADD  CONSTRAINT [DF_PettyCashTransfer_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[PettyCashTransfer] ADD  CONSTRAINT [DF_PettyCashTransfer_Active]  DEFAULT (N'Y') FOR [Active]
GO

 