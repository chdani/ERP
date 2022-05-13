USE [Linkia]
GO

/****** Object:  Table [dbo].[GoodsReceiptNote]    Script Date: 11-Apr-22 11:20:53 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[GoodsReceiptNote](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[PurchaseOrderId] [uniqueidentifier] NOT NULL,
	[VendorMasterId] [uniqueidentifier] NOT NULL,
	[TransNo] [bigint] IDENTITY(1,1) NOT NULL,
	[TransDate] [date] NOT NULL,
	[InvoiceNo] [varchar](50) NOT NULL,
	[InvoiceDate] [date] NOT NULL,
	[WareHouseLocationId] [uniqueidentifier] NULL,
	[Remarks] [nvarchar](1000) NULL,
	[Status] [varchar](20) NOT NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[GoodsReceiptNote] ADD  CONSTRAINT [DF_GoodsReceiptNote_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[GoodsReceiptNote] ADD  CONSTRAINT [DF_GoodsReceiptNote_Active]  DEFAULT (N'Y') FOR [Active]
GO


