USE [Linkia]
GO

/****** Object:  Table [dbo].[DirectInvPostPayment]    Script Date: 18-Feb-22 12:18:31 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DirectInvPostPayment](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[TransNo] [bigint] IDENTITY(1,1) NOT NULL,
	[FinYear] [varchar](20) NOT NULL,
	[OrgId] [uniqueidentifier] NOT NULL,
	[DirInvPrePaymentId] [uniqueidentifier] NOT NULL,
	[VendorMasterId] [uniqueidentifier] NOT NULL,
	[InvoiceNo] [nvarchar](50) NULL,
	[InvoiceDate] [date] NOT NULL,
	[DocumentDate] [date] NOT NULL,
	[LedgerCode] [int] NOT NULL,
	[CostCenterCode] [varchar](20) NOT NULL,
	[Amount] [numeric](20, 6) NOT NULL,
	[Remarks] [nvarchar](300) NULL,
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

ALTER TABLE [dbo].[DirectInvPostPayment] ADD  CONSTRAINT [DF_DirectInvPostPayment_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[DirectInvPostPayment] ADD  CONSTRAINT [DF_DirectInvPostPayment_Active]  DEFAULT (N'Y') FOR [Active]
GO

ALTER TABLE [dbo].[DirectInvPostPayment] ADD  CONSTRAINT [DF__DirectInv__Statu__1C873BEC]  DEFAULT ('SUBMITTED') FOR [Status]
GO


