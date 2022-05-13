USE [Linkia]
GO

/****** Object:  Table [dbo].[EmbPrePaymentEmbDet]    Script Date: 27-Apr-22 1:10:23 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EmbPrePaymentEmbDet](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[EmbPrePaymentHdrId] [uniqueidentifier] NOT NULL,
	[FinYear] [varchar](20) NOT NULL,
	[OrgId] [uniqueidentifier] NOT NULL,
	[Amount] [numeric](20, 6) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL,
	[ClearanceOrdNo] [nvarchar](25) NOT NULL,
	[ClearanceOrdDate] [date] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[EmbPrePaymentEmbDet] ADD  CONSTRAINT [DF_EmbPrePaymentEmbDet_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[EmbPrePaymentEmbDet] ADD  CONSTRAINT [DF_EmbPrePaymentEmbDet_Active]  DEFAULT (N'Y') FOR [Active]
GO


