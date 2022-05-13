USE [Linkia]
GO

/****** Object:  Table [dbo].[BudgAllocHdr]    Script Date: 28-Apr-22 1:32:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BudgAllocHdr](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[TransNo] [bigint] IDENTITY(1,1) NOT NULL,
	[BudgetType] [nvarchar](20) NULL,
	[FinYear] [varchar](20) NOT NULL,
	[OrgId] [uniqueidentifier] NOT NULL,
	[BudgetDate] [date] NOT NULL,
	[BudgetAmount] [numeric](20, 6) NOT NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL,
	[Status] [varchar](20) NOT NULL,
	[ApprovedBy] [uniqueidentifier] NULL,
	[ApprovedDate] [smalldatetime] NULL,
	[ApproverRemarks] [nvarchar](500) NULL,
	[FinBookNo] [nvarchar](20) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[BudgAllocHdr] ADD  CONSTRAINT [DF_BudgAllocHdr_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[BudgAllocHdr] ADD  CONSTRAINT [DF_BudgAllocHdr_Active]  DEFAULT (N'Y') FOR [Active]
GO

ALTER TABLE [dbo].[BudgAllocHdr] ADD  CONSTRAINT [DF__BudgAlloc__Statu__1A9EF37A]  DEFAULT ('SUBMITTED') FOR [Status]
GO


