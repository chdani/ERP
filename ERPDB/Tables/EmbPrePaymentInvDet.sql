USE [Linkia]
GO

/****** Object:  Table [dbo].[EmbPrePaymentInvDet]    Script Date: 27-Apr-22 1:10:37 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EmbPrePaymentInvDet](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[EmbPrePaymentEmbDetId] [uniqueidentifier] NOT NULL,
	[FinYear] [varchar](20) NOT NULL,
	[OrgId] [uniqueidentifier] NOT NULL,
	[InvDate] [date] NULL,
	[InvNo] [nvarchar](50) NULL,
	[Amount] [numeric](20, 6) NOT NULL,
	[DueAmount] [numeric](20, 6) NOT NULL,
	[CurrencyAmount] [numeric](20, 6) NOT NULL,
	[TelexRef] [nvarchar](50) NULL,
	[Remarks] [nvarchar](500) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL,
	[LedgerCode] [int] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[EmbPrePaymentInvDet] ADD  CONSTRAINT [DF_EmbPrePaymentInvDet_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[EmbPrePaymentInvDet] ADD  CONSTRAINT [DF_EmbPrePaymentInvDet_Active]  DEFAULT (N'Y') FOR [Active]
GO


