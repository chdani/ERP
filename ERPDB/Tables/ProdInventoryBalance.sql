USE [Linkia]
GO

/****** Object:  Table [dbo].[ProdInventoryBalance]    Script Date: 11-Apr-22 11:19:53 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProdInventoryBalance](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[WareHouseLocationId] [uniqueidentifier] NOT NULL,
	[ProductMasterId] [uniqueidentifier] NOT NULL,
	[ShelveNo] [varchar](20) NULL,
	[ExpiryDate] [date] NULL,
	[AvlQuantity] [numeric](18, 6) NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProdInventoryBalance] ADD  CONSTRAINT [DF_ProdInventoryBalance_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[ProdInventoryBalance] ADD  CONSTRAINT [DF_ProdInventoryBalance_Active]  DEFAULT (N'Y') FOR [Active]
GO


