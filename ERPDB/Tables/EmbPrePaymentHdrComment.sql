USE [Linkia]
GO

/****** Object:  Table [dbo].[EmbPrePaymentHdrComment]    Script Date: 20-Feb-22 12:49:21 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EmbPrePaymentHdrComment](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[EmbPrePaymentHdrId] [uniqueidentifier] NOT NULL,
	[Comments] [varchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[EmbPrePaymentHdrComment] ADD  CONSTRAINT [DF_EmbPrePaymentHdrComment_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[EmbPrePaymentHdrComment] ADD  CONSTRAINT [DF_EmbPrePaymentHdrComment_Active]  DEFAULT (N'Y') FOR [Active]
GO


