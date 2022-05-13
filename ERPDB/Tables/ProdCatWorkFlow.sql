USE [Linkia]
GO

/****** Object:  Table [dbo].[ProdCatWorkFlow]    Script Date: 09-Mar-22 2:58:23 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProdCatWorkFlow](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ProdCategoryId] [uniqueidentifier] NOT NULL,
	[ApprovalLevel] [int] NOT NULL,
	[ApprovalType] [varchar](20) NOT NULL,
	[ApprovalId] [uniqueidentifier] NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProdCatWorkFlow] ADD  CONSTRAINT [DF_ProdCatWorkFlow_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[ProdCatWorkFlow] ADD  CONSTRAINT [DF_ProdCatWorkFlow_Active]  DEFAULT (N'Y') FOR [Active]
GO


