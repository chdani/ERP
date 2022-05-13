USE [Linkia]
GO

/****** Object:  Table [dbo].[PurchaseOrdComment]    Script Date: 22-Mar-22 2:20:32 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PurchaseOrdComment](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[PurchaseOrderId] [uniqueidentifier] NOT NULL,
	[Comments] [nvarchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PurchaseOrdComment] ADD  CONSTRAINT [DF_PurchaseOrdComment_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[PurchaseOrdComment] ADD  CONSTRAINT [DF_PurchaseOrdComment_Active]  DEFAULT (N'Y') FOR [Active]
GO


