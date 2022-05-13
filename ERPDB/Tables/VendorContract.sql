USE [Linkia]
GO

/****** Object:  Table [dbo].[VendorContract]    Script Date: 09-Mar-22 2:57:11 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[VendorContract](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[VendorMasterId] [uniqueidentifier] NOT NULL,
	[Duration] [int] NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NOT NULL,
	[PaymentTerm] [varchar](20) NOT NULL,
	[LedgerCode] [int] NOT NULL,
	[AmountToHold] [numeric](20, 6) NOT NULL,
	[Description] [nvarchar](300) NOT NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[VendorContract] ADD  CONSTRAINT [DF_VendorContract_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[VendorContract] ADD  CONSTRAINT [DF_VendorContract_Active]  DEFAULT (N'Y') FOR [Active]
GO


