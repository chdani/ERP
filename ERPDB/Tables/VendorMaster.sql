USE [Linkia]
GO

/****** Object:  Table [dbo].[VendorMaster]    Script Date: 09-Mar-22 3:00:38 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[VendorMaster](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Title] [nvarchar](10) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Address1] [nvarchar](100) NOT NULL,
	[Address2] [nvarchar](100) NOT NULL,
	[CountryName] [nvarchar](50) NOT NULL,
	[POBox] [nvarchar](25) NOT NULL,
	[Telephone] [varchar](20) NOT NULL,
	[Mobile] [varchar](20) NOT NULL,
	[Email] [varchar](50) NOT NULL,
	[BankCountryCode] [varchar](50) NOT NULL,
	[BankCode] [varchar](50) NOT NULL,
	[IbanSwifT] [varchar](50) NOT NULL,
	[BankAccName] [nvarchar](50) NOT NULL,
	[BankAccNo] [varchar](50) NOT NULL,
	[LedgerCode] [int] NOT NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[VendorMaster] ADD  CONSTRAINT [DF_VendorMaster_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[VendorMaster] ADD  CONSTRAINT [DF_VendorMaster_Active]  DEFAULT (N'Y') FOR [Active]
GO


