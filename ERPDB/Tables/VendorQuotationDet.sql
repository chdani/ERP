USE [Linkia]
GO

/****** Object:  Table [dbo].[VendorQuotationDet]    Script Date: 17-Mar-22 7:22:51 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[VendorQuotationDet](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[VendorQuotationId] [uniqueidentifier] NOT NULL,
	[ProductMasterId] [uniqueidentifier] NOT NULL,
	[UnitMasterId] [uniqueidentifier] NOT NULL,
	[Price] [numeric](18, 6) NULL,
	[Quantity] [numeric](18, 6) NULL,
	[Amount] [numeric](18, 6) NULL,
	[Remarks] [nvarchar](1000) NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[VendorQuotationDet] ADD  CONSTRAINT [DF_VendorQuotationDet_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[VendorQuotationDet] ADD  CONSTRAINT [DF_VendorQuotationDet_Active]  DEFAULT (N'Y') FOR [Active]
GO


