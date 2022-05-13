USE [Linkia]
GO

/****** Object:  Table [dbo].[QuotationReqDetHist]    Script Date: 22-Mar-22 2:22:12 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[QuotationReqDetHist](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[QuotationReqDetId] [uniqueidentifier] NOT NULL,
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

ALTER TABLE [dbo].[QuotationReqDetHist] ADD  CONSTRAINT [DF_QuotationReqDetHist_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[QuotationReqDetHist] ADD  CONSTRAINT [DF_QuotationReqDetHist_Active]  DEFAULT (N'Y') FOR [Active]
GO


