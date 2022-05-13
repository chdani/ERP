USE [Linkia]
GO

/****** Object:  Table [dbo].[VendorQuotationComment]    Script Date: 22-Mar-22 2:23:23 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[VendorQuotationComment](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[VendorQuotationId] [uniqueidentifier] NOT NULL,
	[Comments] [nvarchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[VendorQuotationComment] ADD  CONSTRAINT [DF_VendorQuotationComment_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[VendorQuotationComment] ADD  CONSTRAINT [DF_VendorQuotationComment_Active]  DEFAULT (N'Y') FOR [Active]
GO


