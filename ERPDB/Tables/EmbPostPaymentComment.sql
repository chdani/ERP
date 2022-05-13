USE [Linkia]
GO

/****** Object:  Table [dbo].[EmbPostPaymentComment]    Script Date: 20-Feb-22 12:48:19 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EmbPostPaymentComment](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[EmbPostPaymentId] [uniqueidentifier] NOT NULL,
	[Comments] [varchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[EmbPostPaymentComment] ADD  CONSTRAINT [DF_EmbPostPaymentComment_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[EmbPostPaymentComment] ADD  CONSTRAINT [DF_EmbPostPaymentComment_Active]  DEFAULT (N'Y') FOR [Active]
GO


