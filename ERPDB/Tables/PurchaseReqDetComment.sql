USE [Linkia]
GO

/****** Object:  Table [dbo].[PurchaseReqDetComment]    Script Date: 22-Mar-22 2:21:32 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PurchaseReqDetComment](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[PurchaseRequestDetId] [uniqueidentifier] NOT NULL,
	[Comments] [nvarchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PurchaseReqDetComment] ADD  CONSTRAINT [DF_PurchaseReqDetComment_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[PurchaseReqDetComment] ADD  CONSTRAINT [DF_PurchaseReqDetComment_Active]  DEFAULT (N'Y') FOR [Active]
GO


