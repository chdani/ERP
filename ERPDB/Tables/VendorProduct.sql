USE [Linkia]
GO

/****** Object:  Table [dbo].[VendorProduct]    Script Date: 13-Mar-22 1:26:24 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[VendorProduct](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[VendorMasterId] [uniqueidentifier] NOT NULL,
	[ProductMasterId] [uniqueidentifier] NOT NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[VendorProduct] ADD  CONSTRAINT [DF_VendorProduct_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[VendorProduct] ADD  CONSTRAINT [DF_VendorProduct_Active]  DEFAULT (N'Y') FOR [Active]
GO


 