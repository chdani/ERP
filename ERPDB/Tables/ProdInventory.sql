USE [Linkia]
GO

/****** Object:  Table [dbo].[ProdInventory]    Script Date: 16-Apr-22 11:32:26 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProdInventory](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[TransId] [uniqueidentifier] NOT NULL,
	[TransType] [varchar](20) NOT NULL,
	[TransNo] [bigint] NULL,
	[TransDate] [date] NOT NULL,
	[WareHouseLocationId] [uniqueidentifier] NOT NULL,
	[ActorType] [varchar](20) NULL,
	[ActorId] [uniqueidentifier] NULL,
	[ProductMasterId] [uniqueidentifier] NOT NULL,
	[ShelveNo] [varchar](20) NULL,
	[Barcode] [varchar](20) NULL,
	[StockIn] [numeric](18, 6) NULL,
	[StockOut] [numeric](18, 6) NULL,
	[ExpiryDate] [date] NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProdInventory] ADD  CONSTRAINT [DF_ProdInventory_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[ProdInventory] ADD  CONSTRAINT [DF_ProdInventory_Active]  DEFAULT (N'Y') FOR [Active]
GO


