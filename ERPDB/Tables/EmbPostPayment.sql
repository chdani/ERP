USE [Linkia]
GO

/****** Object:  Table [dbo].[EmbPostPayment]    Script Date: 22-Feb-22 6:53:15 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EmbPostPayment](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[TransNo] [bigint] IDENTITY(1,1) NOT NULL,
	[EmbassyId] [uniqueidentifier] NOT NULL,
	[FinYear] [varchar](20) NOT NULL,
	[OrgId] [uniqueidentifier] NOT NULL,
	[PaymentDate] [date] NOT NULL,
	[BookNo] [nvarchar](20) NOT NULL,
	[LedgerCode] [int] NOT NULL,
	[Amount] [numeric](20, 6) NOT NULL,
	[CurrencyRate] [numeric](20, 6) NOT NULL,
	[CurrencyAmount] [numeric](20, 6) NOT NULL,
	[CurrencyCode] [nvarchar](10) NOT NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL,
	[Status] [varchar](20) NOT NULL,
	[ApprovedBy] [uniqueidentifier] NULL,
	[ApprovedDate] [smalldatetime] NULL,
	[ApproverRemarks] [nvarchar](500) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[EmbPostPayment] ADD  CONSTRAINT [DF_EmbPostPayment_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[EmbPostPayment] ADD  CONSTRAINT [DF_EmbPostPayment_Active]  DEFAULT (N'Y') FOR [Active]
GO

ALTER TABLE [dbo].[EmbPostPayment] ADD  CONSTRAINT [DF__EmbPostPa__Statu__6225902D]  DEFAULT ('SUBMITTED') FOR [Status]
GO


