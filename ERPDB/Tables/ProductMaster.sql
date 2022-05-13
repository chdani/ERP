USE [Linkia]
GO

/****** Object:  Table [dbo].[ProductMaster]    Script Date: 07-Apr-22 11:28:24 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProductMaster](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ProdCategoryId] [uniqueidentifier] NOT NULL,
	[ProdCode] [nvarchar](100) NOT NULL,
	[ProdDescription] [nvarchar](500) NOT NULL,
	[Barcode] [varchar](100) NOT NULL,
	[ReOrderLevel] [numeric](20, 6) NULL,
	[IsExpirable] [bit] NULL,
	[IsStockable] [bit] NOT NULL,
	[DefaultUnitId] [uniqueidentifier] NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProductMaster] ADD  CONSTRAINT [DF_ProductMaster_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[ProductMaster] ADD  CONSTRAINT [DF__ProductMa__IsExp__2E3BD7D3]  DEFAULT ((0)) FOR [IsExpirable]
GO

ALTER TABLE [dbo].[ProductMaster] ADD  CONSTRAINT [DF_ProductMaster_IsStockable]  DEFAULT ((1)) FOR [IsStockable]
GO

ALTER TABLE [dbo].[ProductMaster] ADD  CONSTRAINT [DF_ProductMaster_Active]  DEFAULT (N'Y') FOR [Active]
GO


