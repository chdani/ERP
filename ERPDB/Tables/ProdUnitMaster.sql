USE [Linkia]
GO

/****** Object:  Table [dbo].[ProdUnitMaster]    Script Date: 27-Mar-22 11:00:27 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProdUnitMaster](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[UnitCode] [nvarchar](100) NOT NULL,
	[UnitName] [nvarchar](500) NOT NULL,
	[ConversionUnit] [numeric](18, 0) NOT NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProdUnitMaster] ADD  CONSTRAINT [DF_ProdUnitMaster_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[ProdUnitMaster] ADD  CONSTRAINT [DF_ProdUnitMaster_ConversionUnit]  DEFAULT ((1)) FOR [ConversionUnit]
GO

ALTER TABLE [dbo].[ProdUnitMaster] ADD  CONSTRAINT [DF_ProdUnitMaster_Active]  DEFAULT (N'Y') FOR [Active]
GO


