USE [Linkia]
GO

/****** Object:  Table [dbo].[LedgerBalDraft]    Script Date: 05-Jan-22 7:10:01 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LedgerBalDraft](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[TransactionId] [uniqueidentifier] NULL,
	[TransactionType] [varchar](20) NULL,
	[OrgId] [uniqueidentifier] NULL,
	[LedgerDate] [date] NOT NULL,
	[FinYear] [varchar](10) NOT NULL,
	[LedgerCode] [int] NOT NULL,
	[Amount] [numeric](20, 6) NOT NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[LedgerBalDraft] ADD  CONSTRAINT [DF_LedgerBalDraft_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[LedgerBalDraft] ADD  CONSTRAINT [DF_LedgerBalDraft_Active]  DEFAULT (N'Y') FOR [Active]
GO


