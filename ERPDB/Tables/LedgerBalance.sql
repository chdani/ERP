USE [Linkia]
GO

/****** Object:  Table [dbo].[LedgerBalance]    Script Date: 01-Feb-22 7:01:04 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LedgerBalance](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[TransactionId] [uniqueidentifier] NULL,
	[TransactionType] [varchar](20) NULL,
	[OrgId] [uniqueidentifier] NULL,
	[LedgerDate] [date] NOT NULL,
	[FinYear] [varchar](10) NOT NULL,
	[LedgerCode] [int] NOT NULL,
	[Credit] [numeric](20, 6) NOT NULL,
	[Debit] [numeric](20, 6) NOT NULL,
	[IsCommitted] [bit] NOT NULL,
	[Remarks] [nvarchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[LedgerBalance] ADD  CONSTRAINT [DF_LedgerBalance_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[LedgerBalance] ADD  CONSTRAINT [DF_LedgerBalance_Credit]  DEFAULT ((0)) FOR [Credit]
GO

ALTER TABLE [dbo].[LedgerBalance] ADD  CONSTRAINT [DF_LedgerBalance_Debit]  DEFAULT ((0)) FOR [Debit]
GO

ALTER TABLE [dbo].[LedgerBalance] ADD  CONSTRAINT [DF_LedgerBalance_IsBlocked]  DEFAULT ((0)) FOR [IsCommitted]
GO

ALTER TABLE [dbo].[LedgerBalance] ADD  CONSTRAINT [DF_LedgerBalance_Active]  DEFAULT (N'Y') FOR [Active]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'On PO creation this will be posted as false, and will change to true once invoice is posted' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LedgerBalance', @level2type=N'COLUMN',@level2name=N'IsCommitted'
GO


