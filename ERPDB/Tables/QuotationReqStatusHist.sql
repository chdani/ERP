USE [Linkia]
GO

/****** Object:  Table [dbo].[QuotationReqStatusHist]    Script Date: 07-Apr-22 2:17:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[QuotationReqStatusHist](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[QuotationRequestId] [uniqueidentifier] NOT NULL,
	[Status] [varchar](20) NOT NULL,
	[Comments] [nvarchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[QuotationReqStatusHist] ADD  CONSTRAINT [DF_QuotationReqStatusHist_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[QuotationReqStatusHist] ADD  CONSTRAINT [DF_QuotationReqStatusHist_Active]  DEFAULT (N'Y') FOR [Active]
GO


