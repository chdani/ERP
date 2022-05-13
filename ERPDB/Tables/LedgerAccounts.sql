USE [Linkia]
GO

/****** Object:  Table [dbo].[LedgerAccounts]    Script Date: 01-Feb-22 3:37:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LedgerAccounts](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[LedgerCode] [int] NOT NULL,
	[LedgerDesc] [nvarchar](100) NOT NULL,
	[UsedFor] [varchar](20) NULL,
	[Remarks] [nvarchar](1000) NULL,
	[OrgId] [uniqueidentifier] NOT NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[LedgerAccounts] ADD  CONSTRAINT [DF_LedgerAccounts_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[LedgerAccounts] ADD  CONSTRAINT [DF_LedgerAccounts_IsBudgetable]  DEFAULT ((0)) FOR [UsedFor]
GO

ALTER TABLE [dbo].[LedgerAccounts] ADD  CONSTRAINT [DF_LedgerAccounts_Active]  DEFAULT (N'Y') FOR [Active]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'BA - Used for Budget Allocation, CR - Cash Receipt Bank Account, OT - Others' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LedgerAccounts', @level2type=N'COLUMN',@level2name=N'UsedFor'
GO


