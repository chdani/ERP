USE [Linkia]
GO

/****** Object:  Table [dbo].[PurchaseOrder]    Script Date: 21-Apr-22 7:05:15 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PurchaseOrder](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[PurchaseRequestId] [uniqueidentifier] NOT NULL,
	[VendorMasterId] [uniqueidentifier] NOT NULL,
	[TransNo] [bigint] IDENTITY(1,1) NOT NULL,
	[TransDate] [date] NOT NULL,
	[AccountCode] [int] NOT NULL,
	[Remarks] [nvarchar](1000) NULL,
	[Status] [varchar](20) NOT NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL,
	[LedgerCode] [int] NULL,
	[CostCenterCode] [varchar](20) NULL,
	[FinYear] [varchar](20) NULL,
	[TotalAmount] [numeric](18, 6) NULL,
	[OrgId] [uniqueidentifier] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PurchaseOrder] ADD  CONSTRAINT [DF_PurchaseOrder_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[PurchaseOrder] ADD  CONSTRAINT [DF_PurchaseOrder_Active]  DEFAULT (N'Y') FOR [Active]
GO


