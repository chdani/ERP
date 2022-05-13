USE [Linkia]
GO

/****** Object:  Table [dbo].[GoodReceiptNoteDet]    Script Date: 11-Apr-22 11:20:27 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[GoodReceiptNoteDet](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[GoodsReceiptNoteId] [uniqueidentifier] NOT NULL,
	[ProductMasterId] [uniqueidentifier] NOT NULL,
	[Barcode] [varchar](20) NOT NULL,
	[ShelveNo] [varchar](20) NULL,
	[UnitMasterId] [uniqueidentifier] NOT NULL,
	[Price] [numeric](18, 6) NULL,
	[Quantity] [numeric](18, 6) NULL,
	[Amount] [numeric](18, 6) NULL,
	[ExpiryDate] [date] NULL,
	[Remarks] [nvarchar](1000) NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[GoodReceiptNoteDet] ADD  CONSTRAINT [DF_GoodReceiptNoteDet_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[GoodReceiptNoteDet] ADD  CONSTRAINT [DF_GoodReceiptNoteDet_Active]  DEFAULT (N'Y') FOR [Active]
GO


