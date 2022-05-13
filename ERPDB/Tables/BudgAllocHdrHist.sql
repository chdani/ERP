USE [Linkia]
GO

/****** Object:  Table [dbo].[BudgAllocHdr]    Script Date: 14-Feb-22 1:31:28 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BudgAllocHdrHist](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[BudgAllocHdrId] [uniqueidentifier] NOT NULL,
	[FieldName] [varchar](50) NOT NULL,
	[PrevValue] [nvarchar](max),
	[CurrentValue] nvarchar(max),
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[BudgAllocHdrHist] ADD  CONSTRAINT [DF_BudgAllocHdrHist_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[BudgAllocHdrHist] ADD  CONSTRAINT [DF_BudgAllocHdrHist_Active]  DEFAULT (N'Y') FOR [Active]
GO

 

