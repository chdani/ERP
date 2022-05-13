USE [Linkia]
GO

/****** Object:  Table [dbo].[BudgAllocHdrStatusHist]    Script Date: 18-Feb-22 12:20:21 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BudgAllocHdrStatusHist](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[BudgAllocHdrId] [uniqueidentifier] NOT NULL,
	[Status] [varchar](20) NOT NULL,
	[Comments] [varchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[BudgAllocHdrStatusHist] ADD  CONSTRAINT [DF_BudgAllocHdrStatusHist_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[BudgAllocHdrStatusHist] ADD  CONSTRAINT [DF_BudgAllocHdrStatusHist_Active]  DEFAULT (N'Y') FOR [Active]
GO


