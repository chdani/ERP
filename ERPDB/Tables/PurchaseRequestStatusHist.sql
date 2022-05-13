USE [Linkia]
GO

/****** Object:  Table [dbo].[PurchaseRequestStatusHist]    Script Date: 22-Mar-22 2:21:57 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PurchaseRequestStatusHist](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[PurchaseRequestId] [uniqueidentifier] NOT NULL,
	[Status] [varchar](20) NOT NULL,
	[Comments] [nvarchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PurchaseRequestStatusHist] ADD  CONSTRAINT [DF_PurchaseRequestStatusHist_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[PurchaseRequestStatusHist] ADD  CONSTRAINT [DF_PurchaseRequestStatusHist_Active]  DEFAULT (N'Y') FOR [Active]
GO


