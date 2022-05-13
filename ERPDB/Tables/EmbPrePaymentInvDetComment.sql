USE [Linkia]
GO

/****** Object:  Table [dbo].[EmbPrePaymentInvDetComment]    Script Date: 20-Feb-22 12:49:49 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EmbPrePaymentInvDetComment](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[EmbPrePaymentInvDetId] [uniqueidentifier] NOT NULL,
	[Comments] [varchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[EmbPrePaymentInvDetComment] ADD  CONSTRAINT [DF_EmbPrePaymentInvDetComment_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[EmbPrePaymentInvDetComment] ADD  CONSTRAINT [DF_EmbPrePaymentInvDetComment_Active]  DEFAULT (N'Y') FOR [Active]
GO


