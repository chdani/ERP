USE [Linkia]
GO

/****** Object:  Table [dbo].[EmbPostPaymentHist]    Script Date: 20-Feb-22 12:48:26 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EmbPostPaymentHist](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[EmbPostPaymentId] [uniqueidentifier] NOT NULL,
	[FieldName] [varchar](50) NOT NULL,
	[PrevValue] [nvarchar](max) NULL,
	[CurrentValue] [nvarchar](max) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[EmbPostPaymentHist] ADD  CONSTRAINT [DF_EmbPostPaymentHist_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[EmbPostPaymentHist] ADD  CONSTRAINT [DF_EmbPostPaymentHist_Active]  DEFAULT (N'Y') FOR [Active]
GO


