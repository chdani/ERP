USE [Linkia]
GO

/****** Object:  Table [dbo].[VendorQuotationHist]    Script Date: 22-Mar-22 2:23:47 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[VendorQuotationHist](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[VendorQuotationId] [uniqueidentifier] NOT NULL,
	[FieldName] [varchar](50) NOT NULL,
	[PrevValue] [nvarchar](max) NULL,
	[CurrentValue] [nvarchar](max) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[VendorQuotationHist] ADD  CONSTRAINT [DF_VendorQuotationHist_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[VendorQuotationHist] ADD  CONSTRAINT [DF_VendorQuotationHist_Active]  DEFAULT (N'Y') FOR [Active]
GO


