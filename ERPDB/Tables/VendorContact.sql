USE [Linkia]
GO

/****** Object:  Table [dbo].[VendorContact]    Script Date: 09-Mar-22 2:57:03 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[VendorContact](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[VendorMasterId] [uniqueidentifier] NOT NULL,
	[EmailId] [varchar](200) NOT NULL,
	[MobileNo] [varchar](200) NOT NULL,
	[ContactName] [varchar](200) NOT NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[VendorContact] ADD  CONSTRAINT [DF_VendorContact_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[VendorContact] ADD  CONSTRAINT [DF_VendorContact_Active]  DEFAULT (N'Y') FOR [Active]
GO


