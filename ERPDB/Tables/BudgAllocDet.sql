USE [Linkia]
GO

/****** Object:  Table [dbo].[BudgAllocDet]    Script Date: 01-Feb-22 6:54:16 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BudgAllocDet](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[BudgAllocHdrId] [uniqueidentifier] NOT NULL,
	[OrgId] [uniqueidentifier] NOT NULL,
	[LedgerCode] [int] NOT NULL,
	[ToLedgerCode] [int] NULL,
	[BudgetAmount] [numeric](20, 6) NOT NULL,
	[Remarks] [nvarchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[BudgAllocDet] ADD  CONSTRAINT [DF_BudgAllocDet_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[BudgAllocDet] ADD  CONSTRAINT [DF_BudgAllocDet_Active]  DEFAULT (N'Y') FOR [Active]
GO


