USE [Linkia]
GO

/****** Object:  Table [dbo].[DirectInvPrePayment]    Script Date: 21-Apr-22 7:04:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DirectInvPrePayment](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[TransNo] [bigint] IDENTITY(1,1) NOT NULL,
	[FinYear] [varchar](20) NOT NULL,
	[OrgId] [uniqueidentifier] NOT NULL,
	[VendorMasterId] [uniqueidentifier] NOT NULL,
	[InvoiceNo] [nvarchar](50) NULL,
	[InvoiceDate] [date] NULL,
	[DocumentDate] [date] NOT NULL,
	[LedgerCode] [int] NOT NULL,
	[CostCenterCode] [varchar](20) NOT NULL,
	[Amount] [numeric](20, 6) NOT NULL,
	[DueAmount] [numeric](20, 6) NOT NULL,
	[Remarks] [nvarchar](300) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL,
	[Status] [varchar](20) NOT NULL,
	[ApprovedBy] [uniqueidentifier] NULL,
	[ApprovedDate] [smalldatetime] NULL,
	[ApproverRemarks] [nvarchar](500) NULL,
	[PurchaseOrderId] [uniqueidentifier] NULL,
	[DocumentNo] [bigint] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DirectInvPrePayment] ADD  CONSTRAINT [DF_DirectInvPrePayment_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[DirectInvPrePayment] ADD  CONSTRAINT [DF_DirectInvPrePayment_Active]  DEFAULT (N'Y') FOR [Active]
GO

ALTER TABLE [dbo].[DirectInvPrePayment] ADD  CONSTRAINT [DF__DirectInv__Statu__1D7B6025]  DEFAULT ('SUBMITTED') FOR [Status]
GO


