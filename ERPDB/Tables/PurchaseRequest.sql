USE [Linkia]
GO

/****** Object:  Table [dbo].[PurchaseRequest]    Script Date: 27-Mar-22 4:47:09 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PurchaseRequest](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[VendorQuotationId] [uniqueidentifier] NOT NULL,
	[VendorMasterId] [uniqueidentifier] NOT NULL,
	[TransDate] [date] NOT NULL,
	[TransNo] [bigint] IDENTITY(1,1) NOT NULL,
	[Remarks] [nvarchar](1000) NULL,
	[Status] [varchar](50) NOT NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PurchaseRequest] ADD  CONSTRAINT [DF_PurchaseRequest_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[PurchaseRequest] ADD  CONSTRAINT [DF_PurchaseRequest_Active]  DEFAULT (N'Y') FOR [Active]
GO


