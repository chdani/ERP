USE [Linkia]
GO

/****** Object:  Table [dbo].[VendorQuotation]    Script Date: 17-Mar-22 7:22:42 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[VendorQuotation](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[QuotationRequestId] [uniqueidentifier] NOT NULL,
	[VendorMasterId] [uniqueidentifier] NOT NULL,
	[TransNo] [bigint] IDENTITY(1,1) NOT NULL,
	[TransDate] [date] NOT NULL,
	[QuotationNo] [varchar](50) NOT NULL,
	[QuotationdDate] [date] NOT NULL,
	[IsApproved] [bit] NOT NULL,
	[Remarks] [nvarchar](1000) NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[VendorQuotation] ADD  CONSTRAINT [DF_VendorQuotation_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[VendorQuotation] ADD  CONSTRAINT [DF__VendorQuo__IsApp__34E8D562]  DEFAULT ((0)) FOR [IsApproved]
GO

ALTER TABLE [dbo].[VendorQuotation] ADD  CONSTRAINT [DF_VendorQuotation_Active]  DEFAULT (N'Y') FOR [Active]
GO


