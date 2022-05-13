USE [Linkia]
GO

/****** Object:  Table [dbo].[EmbPrePaymentHdr]    Script Date: 27-Apr-22 1:10:29 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EmbPrePaymentHdr](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[TransNo] [bigint] IDENTITY(1,1) NOT NULL,
	[FinYear] [varchar](20) NOT NULL,
	[OrgId] [uniqueidentifier] NOT NULL,
	[BookDate] [date] NOT NULL,
	[BookNo] [nvarchar](20) NOT NULL,
	[Status] [varchar](20) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[ApprovedBy] [uniqueidentifier] NULL,
	[ApprovedDate] [smalldatetime] NULL,
	[ApproverRemarks] [nvarchar](500) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL,
	[EmbassyId] [uniqueidentifier] NOT NULL,
	[CurrencyCode] [nvarchar](10) NULL,
	[CurrencyRate] [numeric](20, 6) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[EmbPrePaymentHdr] ADD  CONSTRAINT [DF_EmpassyPaymentHdr_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[EmbPrePaymentHdr] ADD  CONSTRAINT [DF_EmbPrePaymentHdr_Status]  DEFAULT ('SUBMITTED') FOR [Status]
GO

ALTER TABLE [dbo].[EmbPrePaymentHdr] ADD  CONSTRAINT [DF_EmpassyPaymentHdr_Active]  DEFAULT (N'Y') FOR [Active]
GO


