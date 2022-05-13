USE [Linkia]
GO

/****** Object:  Table [dbo].[BudgAllocDetComment]    Script Date: 20-Feb-22 12:47:42 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BudgAllocDetComment](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[BudgAllocDetId] [uniqueidentifier] NOT NULL,
	[Comments] [varchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[BudgAllocDetComment] ADD  CONSTRAINT [DF_BudgAllocDetComment_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[BudgAllocDetComment] ADD  CONSTRAINT [DF_BudgAllocDetComment_Active]  DEFAULT (N'Y') FOR [Active]
GO


