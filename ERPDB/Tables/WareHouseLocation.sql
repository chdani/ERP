USE [Linkia]
GO

/****** Object:  Table [dbo].[WareHouseLocation]    Script Date: 13-Mar-22 12:30:48 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WareHouseLocation](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[WarehouseId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Address] [nvarchar](500) NOT NULL,
	[Email] [nvarchar](100) NULL,
	[ContactNo] [nvarchar](20) NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[WareHouseLocation] ADD  CONSTRAINT [DF_WareHouseLocation_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[WareHouseLocation] ADD  CONSTRAINT [DF_WareHouseLocation_Active]  DEFAULT (N'Y') FOR [Active]
GO


