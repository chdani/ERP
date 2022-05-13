USE [Linkia]
GO

/****** Object:  Table [dbo].[VendorQuotationDetComment]    Script Date: 22-Mar-22 2:23:32 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[VendorQuotationDetComment](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[VendorQuotationDetId] [uniqueidentifier] NOT NULL,
	[Comments] [nvarchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[VendorQuotationDetComment] ADD  CONSTRAINT [DF_VendorQuotationDetComment_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[VendorQuotationDetComment] ADD  CONSTRAINT [DF_VendorQuotationDetComment_Active]  DEFAULT (N'Y') FOR [Active]
GO


