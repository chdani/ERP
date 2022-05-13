USE [Linkia]
GO

/****** Object:  Table [dbo].[CashTransacion]    Script Date: 21-Dec-21 10:21:49 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CashTransacion](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[TellerId] [uniqueidentifier] NOT NULL,
	[TellerUserId] [uniqueidentifier] NOT NULL,
	[AccountId] [uniqueidentifier] NOT NULL,
	[OrgId] [uniqueidentifier] NOT NULL,
	[ProcessType] [varchar](20) NOT NULL,
	[TransType] [varchar](20) NOT NULL,
	[TransDate] [smalldatetime] NOT NULL,
	[LedgerCode] [int] NOT NULL,
	[Recipient] [nvarchar](500) NOT NULL,
	[CostCenter] [varchar](20) NOT NULL,
	[FinYear] [varchar](20) NOT NULL,
	[Credit] [numeric](20, 6) NOT NULL,
	[Debit] [numeric](20, 6) NOT NULL,
	[ReferenceNo] [nvarchar](50) NOT NULL,
	[Remarks] [nvarchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CashTransacion] ADD  CONSTRAINT [DF_CashTransacion_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[CashTransacion] ADD  CONSTRAINT [DF_CashTransacion_Active]  DEFAULT (N'Y') FOR [Active]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'''E''  - Expense, R - Receipts' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CashTransacion', @level2type=N'COLUMN',@level2name=N'TransType'
GO


