USE [Linkia]
GO

/****** Object:  Table [dbo].[QuotationReqDet]    Script Date: 17-Mar-22 7:22:06 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[QuotationReqDet](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[QuotationRequestId] [uniqueidentifier] NOT NULL,
	[ProductMasterId] [uniqueidentifier] NOT NULL,
	[Quantity] [numeric](18, 6) NULL,
	[UnitMasterId] [uniqueidentifier] NOT NULL,
	[Remarks] [nvarchar](1000) NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[QuotationReqDet] ADD  CONSTRAINT [DF_QuotationReqDet_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[QuotationReqDet] ADD  CONSTRAINT [DF_QuotationReqDet_Active]  DEFAULT (N'Y') FOR [Active]
GO


