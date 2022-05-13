USE [Linkia]
GO

/****** Object:  Table [dbo].[InventoryTransferDet]    Script Date: 17-Apr-22 11:26:18 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[InventoryTransferDet](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[InventoryTransferId] [uniqueidentifier] NOT NULL,
	[ProductMasterId] [uniqueidentifier] NOT NULL,
	[UnitMasterId] [uniqueidentifier] NOT NULL,
	[Barcode] [varchar](20) NULL,
	[ShelveNo] [varchar](20) NULL,
	[Quantity] [numeric](18, 6) NULL,
	[Remarks] [nvarchar](2000) NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[InventoryTransferDet] ADD  CONSTRAINT [DF_InventoryTransferDet_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[InventoryTransferDet] ADD  CONSTRAINT [DF_InventoryTransferDet_Active]  DEFAULT (N'Y') FOR [Active]
GO


