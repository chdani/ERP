USE [Linkia]
GO

/****** Object:  Table [dbo].[PurchaseOrdDetComment]    Script Date: 22-Mar-22 2:20:39 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PurchaseOrdDetComment](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[PurchaseOrderDetId] [uniqueidentifier] NOT NULL,
	[Comments] [nvarchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PurchaseOrdDetComment] ADD  CONSTRAINT [DF_PurchaseOrdDetComment_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[PurchaseOrdDetComment] ADD  CONSTRAINT [DF_PurchaseOrdDetComment_Active]  DEFAULT (N'Y') FOR [Active]
GO


