USE [Linkia]
GO

/****** Object:  Table [dbo].[EmbPostPaymentStatusHist]    Script Date: 20-Feb-22 12:48:38 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EmbPostPaymentStatusHist](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[EmbPostPaymentId] [uniqueidentifier] NOT NULL,
	[Status] [varchar](20) NOT NULL,
	[Comments] [varchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[EmbPostPaymentStatusHist] ADD  CONSTRAINT [DF_EmbPostPaymentStatusHist_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[EmbPostPaymentStatusHist] ADD  CONSTRAINT [DF_EmbPostPaymentStatusHist_Active]  DEFAULT (N'Y') FOR [Active]
GO


