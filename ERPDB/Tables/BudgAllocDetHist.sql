USE [Linkia]
GO

/****** Object:  Table [dbo].[BudgAllocDetHist]    Script Date: 20-Feb-22 12:47:51 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BudgAllocDetHist](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[BudgAllocDetId] [uniqueidentifier] NOT NULL,
	[FieldName] [varchar](50) NOT NULL,
	[PrevValue] [nvarchar](max) NULL,
	[CurrentValue] [nvarchar](max) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[BudgAllocDetHist] ADD  CONSTRAINT [DF_BudgAllocDetHist_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[BudgAllocDetHist] ADD  CONSTRAINT [DF_BudgAllocDetHist_Active]  DEFAULT (N'Y') FOR [Active]
GO


