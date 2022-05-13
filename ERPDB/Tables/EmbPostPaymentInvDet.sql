USE [Linkia]
GO

/****** Object:  Table [dbo].[EmbPostPaymentInvDet]    Script Date: 22-Feb-22 6:53:34 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EmbPostPaymentInvDet](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[EmbPostPaymentId] [uniqueidentifier] NOT NULL,
	[EmbPrePaymentInvDetId] [uniqueidentifier] NOT NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[EmbPostPaymentInvDet] ADD  CONSTRAINT [DF_EmbPostPaymentInvDet_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[EmbPostPaymentInvDet] ADD  CONSTRAINT [DF_EmbPostPaymentInvDet_Active]  DEFAULT (N'Y') FOR [Active]
GO


 