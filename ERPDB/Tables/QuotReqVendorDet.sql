USE [Linkia]
GO

/****** Object:  Table [dbo].[QuotReqVendorDet]    Script Date: 17-Mar-22 7:25:10 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[QuotReqVendorDet](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[VendorMasterId] [uniqueidentifier] NOT NULL,
	[QuotationRequestId] [uniqueidentifier] NOT NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[QuotReqVendorDet] ADD  CONSTRAINT [DF_QuotReqVendorDet_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[QuotReqVendorDet] ADD  CONSTRAINT [DF_QuotReqVendorDet_Active]  DEFAULT (N'Y') FOR [Active]
GO


