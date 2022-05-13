USE [Linkia]
GO

/****** Object:  Table [dbo].[QuotationRequestComment]    Script Date: 22-Mar-22 2:22:19 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[QuotationRequestComment](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[QuotationRequestId] [uniqueidentifier] NOT NULL,
	[Comments] [nvarchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[QuotationRequestComment] ADD  CONSTRAINT [DF_QuotationRequestComment_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[QuotationRequestComment] ADD  CONSTRAINT [DF_QuotationRequestComment_Active]  DEFAULT (N'Y') FOR [Active]
GO


